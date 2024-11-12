using Game.Data;
using SadUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PhotonMovementManager : Singleton<PhotonMovementManager>
    {
        // Expressed in tiles/second
        [field: SerializeField] public float moveSpeed { get; private set; }

        private GridData openGrid;

        private Dictionary<Photon, Coroutine> runningRoutines;
        private WaitForSeconds waitForMoveTile;

        #region Awake / Destroy
        protected override void Awake()
        {
            runningRoutines = new();
            waitForMoveTile = new WaitForSeconds(1f / moveSpeed);

            SetInstance(this);
            SetupListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void SetupListeners()
        {
            SimulationManager.OnSimulationInitialize += SimulationManager_OnSimulationInitialize;
            SimulationManager.OnSimulationStop += SimulationManager_OnSimulationStop;
            OpticComponent.OnPhotonExit += OpticComponent_OnPhotonExit;
        }

        private void RemoveListeners()
        {
            SimulationManager.OnSimulationInitialize -= SimulationManager_OnSimulationInitialize;
            SimulationManager.OnSimulationStop -= SimulationManager_OnSimulationStop;
            OpticComponent.OnPhotonExit -= OpticComponent_OnPhotonExit;
        }
        #endregion

        #region Handle Events
        private void SimulationManager_OnSimulationInitialize() => HandleSimulationInit();
        private void SimulationManager_OnSimulationStop() => HandleSimulationStop();
        private void OpticComponent_OnPhotonExit(Photon photon) => HandlePhotonExit(photon);

        private void HandleSimulationInit()
        {
            openGrid = GridManager.Instance.GetActiveGrid();
        }

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
                openGrid,
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

            while (photonPos != targetPos)
            {
                yield return waitForMoveTile;

                photonPos += propagation;
                photon.SetPosition(photonPos);
            }

            port.ProcessPhoton(photon);
            runningRoutines.Remove(photon);
        }

        private IEnumerator MovePhotonToGridEdgeCo(Photon photon)
        {
            Vector2Int photonPos = photon.GetPosition();
            Orientation propagation = photon.GetPropagation();
            Vector2Int propagationVector = propagation.ToVector2Int();

            while (IsPositionInGrid(photonPos, propagation))
            {
                yield return waitForMoveTile;

                photonPos += propagationVector;
                photon.SetPosition(photonPos);
            }

            runningRoutines.Remove(photon);
            // DELETE PHOTON HERE
        }

        private bool IsPositionInGrid(Vector2Int position, Orientation propagation)
        {
            return propagation switch
            {
                Orientation.Up => position.y < openGrid.size.y,
                Orientation.Right => position.x < openGrid.size.x,
                Orientation.Down => position.y >= 0,
                Orientation.Left => position.x >= 0,
                _ => true
            };
        }
        #endregion
    }
}
