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
        [field: SerializeField, Range(1, 10)] public float MoveSpeed { get; private set; }
        [field: SerializeField, Range(1,10)] public int ClassicSpeedMultiplier { get; private set; }

        private Dictionary<Photon, Coroutine> runningRoutines;

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

            if(!runningRoutines.ContainsKey(photon)) runningRoutines.Add(photon, routine);
        }
        #endregion

        #region Move Photon Routine
        private IEnumerator MovePhotonToTargetCo(Photon photon, ComponentPort port)
        {
            Vector2Int photonPos = photon.GetPosition();
            Vector2Int propagation = photon.GetPropagationIntVector();
            Vector2Int targetPos = port.position;

            while (photonPos != targetPos)
            {
                yield return GetTimeToWaitBeforeMove(photonPos, targetPos, photon.GetPhotonType());

                photonPos += propagation;
                photon.SetPosition(photonPos);

                bool reachedTarget = photonPos == targetPos;
                OnPhotonDisplace?.Invoke(photon, reachedTarget);
            }
            if (!runningRoutines.ContainsKey(photon)) yield break;

            runningRoutines.Remove(photon);

            if (!port.IsGhostPort)
                port.ProcessPhoton(photon);
            else
                PhotonManager.RemovePhoton(photon, false);
        }

        private WaitForSeconds GetTimeToWaitBeforeMove(Vector2Int currentPos, Vector2Int targetPos, PhotonType type)
        {
            WaitForSeconds waitSeconds = GetWaitMoveTime(type);

            if(type == PhotonType.Classical)
                waitSeconds = new(1f / ClassicCombinedSpeed());

            if (Vector2Int.Distance(currentPos, targetPos) <= 1f)
            {
                waitSeconds = GetWaitMoveTime(type, true); 
                if (type == PhotonType.Classical)
                    waitSeconds = new(1f / (ClassicCombinedSpeed() * 2f));
            }
            return waitSeconds;
        }


        public WaitForSeconds GetWaitMoveTime(PhotonType type, bool half = false, float tiles = 1)
        {
            float waitValue = 1f / MoveSpeed;
            if(type == PhotonType.Classical) waitValue = 1f / ClassicCombinedSpeed();
            if (half) waitValue /= 2;

            return new WaitForSeconds(waitValue * tiles);
        }

        public float ClassicCombinedSpeed()
        {
            return MoveSpeed * ClassicSpeedMultiplier;
        }

        private IEnumerator MovePhotonToGridEdgeCo(Photon photon)
        {
            Vector2Int photonPos = photon.GetPosition();
            Orientation propagation = photon.GetPropagation();
            Vector2Int propagationVector = propagation.ToVector2Int();
            Vector2Int gridSize = photon.currentGrid.size;

            while (IsPositionInGrid(photonPos, propagation, gridSize))
            {
                yield return GetWaitMoveTime(photon.GetPhotonType());

                photonPos += propagationVector;
                photon.SetPosition(photonPos);
                OnPhotonDisplace?.Invoke(photon, !IsPositionInGrid(photonPos, propagation, gridSize));
            }

            if (!runningRoutines.ContainsKey(photon)) yield break;

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

        #region Photon destroy handling
        public void RemovePhotonRoutine(Photon photon)
        {
            if (!runningRoutines.ContainsKey(photon)) return;
            StopCoroutine(runningRoutines[photon]);
            runningRoutines.Remove(photon);
        }
        #endregion

        PhotonManager PhotonManager => PhotonManager.Instance;
    }
}
