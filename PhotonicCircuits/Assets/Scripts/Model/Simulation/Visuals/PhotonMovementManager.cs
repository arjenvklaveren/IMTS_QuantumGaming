using Game.Data;
using SadUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PhotonMovementManager : Singleton<PhotonMovementManager>
    {
        /// <summary>
        /// Passes the photon displaced and if the displacement reaches the destination.
        /// </summary>
        public event Action<Photon, bool> OnPhotonDisplace;

        // Expressed in tiles/second
        [field: SerializeField] public float MoveSpeed { get; private set; }

        private Dictionary<Photon, Coroutine> runningRoutines;

        public WaitForSeconds WaitForMoveTile { get; private set; }
        public WaitForSeconds WaitForMoveHalfTile { get; private set; }


        #region Awake / Destroy
        protected override void Awake()
        {
            SetDefaultValues();

            SetInstance(this);
            SetupListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void SetDefaultValues()
        {
            runningRoutines = new();

            WaitForMoveTile = new(1f / MoveSpeed);
            WaitForMoveHalfTile = new(1f / (MoveSpeed * 2f));
        }

        private void SetupListeners()
        {
            SimulationManager.OnSimulationStop += SimulationManager_OnSimulationStop;
            OpticComponent.OnPhotonExit += OpticComponent_OnPhotonExit;
        }

        private void RemoveListeners()
        {
            SimulationManager.OnSimulationStop -= SimulationManager_OnSimulationStop;
            OpticComponent.OnPhotonExit -= OpticComponent_OnPhotonExit;
        }
        #endregion

        #region Handle Events
        private void SimulationManager_OnSimulationStop() => HandleSimulationStop();
        private void OpticComponent_OnPhotonExit(Photon photon) => HandlePhotonExit(photon);

        private void HandleSimulationStop()
        {
            foreach (KeyValuePair<Photon, Coroutine> pair in runningRoutines)
                StopCoroutine(pair.Value);

            runningRoutines.Clear();
        }

        private void HandlePhotonExit(Photon photon)
        {
            bool hasTarget = PhotonPathFinder.TryFindNextInPort(
                photon.GetPosition(),
                photon.GetPropagation(),
                photon.currentGrid,
                out ComponentPort port);

            Coroutine routine;

            if (hasTarget)
                routine = StartCoroutine(MovePhotonToTargetCo(photon, port));
            else
                routine = StartCoroutine(MovePhotonToGridEdgeCo(photon));

            runningRoutines.Add(photon, routine);
        }
        #endregion

        #region Move Photon Routine
        private IEnumerator MovePhotonToTargetCo(Photon photon, ComponentPort port)
        {
            Vector2Int photonPos = photon.GetPosition();
            Vector2Int propagation = photon.GetPropagationIntVector();
            Vector2Int targetPos = port.position;
            bool isGhostPort = port.IsGhostPort;

            while (photonPos != targetPos)
            {
                yield return GetTimeToWait(photonPos, targetPos);

                photonPos += propagation;
                photon.SetPosition(photonPos);
                OnPhotonDisplace?.Invoke(photon, photonPos == targetPos);
            }

            runningRoutines.Remove(photon);

            if (!isGhostPort)
                port.ProcessPhoton(photon);

            if (isGhostPort)
                PhotonManager.RemovePhoton(photon, false);
        }

        private WaitForSeconds GetTimeToWait(Vector2Int currentPos, Vector2Int targetPos)
        {
            if (Vector2Int.Distance(currentPos, targetPos) <= 1f)
                return WaitForMoveHalfTile;

            return WaitForMoveTile;
        }

        private IEnumerator MovePhotonToGridEdgeCo(Photon photon)
        {
            Vector2Int photonPos = photon.GetPosition();
            Orientation propagation = photon.GetPropagation();
            Vector2Int propagationVector = propagation.ToVector2Int();
            Vector2Int gridSize = photon.currentGrid.size;

            while (IsPositionInGrid(photonPos, propagation, gridSize))
            {
                yield return WaitForMoveTile;

                photonPos += propagationVector;
                photon.SetPosition(photonPos);
                OnPhotonDisplace?.Invoke(photon, !IsPositionInGrid(photonPos, propagation, gridSize));
            }

            runningRoutines.Remove(photon);
            PhotonManager.RemovePhoton(photon, false);
        }

        private bool IsPositionInGrid(Vector2Int position, Orientation propagation, Vector2Int gridSize)
        {
            return propagation switch
            {
                Orientation.Up => position.y < gridSize.y,
                Orientation.Right => position.x < gridSize.x,
                Orientation.Down => position.y >= 0,
                Orientation.Left => position.x >= 0,
                _ => true
            };
        }
        #endregion

        PhotonManager PhotonManager => PhotonManager.Instance;
    }
}
