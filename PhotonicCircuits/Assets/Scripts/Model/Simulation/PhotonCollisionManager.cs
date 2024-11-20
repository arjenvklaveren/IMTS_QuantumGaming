using Game.Data;
using SadUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PhotonCollisionManager : Singleton<PhotonCollisionManager>
    {
        private Dictionary<Vector2Int, List<Photon>> recentPhotonDisplacements;

        private Coroutine clearRoutine;

        #region Awake / Destroy
        protected override void Awake()
        {
            SetInstance(this);

            recentPhotonDisplacements = new();
        }

        private IEnumerator Start()
        {
            yield return PhotonMovementManager.WaitForInstance;
            SetupListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void SetupListeners()
        {
            PhotonMovementManager.Instance.OnPhotonDisplace += PhotonMovementManger_OnPhotonDisplace;
            SimulationManager.OnSimulationInitialize += SimulationManager_OnSimulationInitialize;
            SimulationManager.OnSimulationStop += SimulationManager_OnSimulationStop;
        }

        private void RemoveListeners()
        {
            PhotonMovementManager.Instance.OnPhotonDisplace -= PhotonMovementManger_OnPhotonDisplace;
            SimulationManager.OnSimulationInitialize -= SimulationManager_OnSimulationInitialize;
            SimulationManager.OnSimulationStop -= SimulationManager_OnSimulationStop;
        }
        #endregion

        #region Handle Events
        private void PhotonMovementManger_OnPhotonDisplace(Photon photon) => RegisterPhotonDisplacement(photon);

        private void SimulationManager_OnSimulationInitialize()
        {
            clearRoutine = StartCoroutine(ClearDisplacementsCo());
        }

        private void SimulationManager_OnSimulationStop()
        {
            StopCoroutine(clearRoutine);
        }
        #endregion

        #region Register Photon
        private void RegisterPhotonDisplacement(Photon photon)
        {
            Vector2Int photonPos = photon.GetPosition();

            if (!recentPhotonDisplacements.ContainsKey(photonPos))
                recentPhotonDisplacements.Add(photonPos, new());

            recentPhotonDisplacements[photonPos].Add(photon);

            if (recentPhotonDisplacements[photonPos].Count > 2)
                HandlePhotonCollisionsAtPosition(photonPos);
        }
        #endregion

        #region Handle Photon Collision
        private void HandlePhotonCollisionsAtPosition(Vector2Int pos)
        {
            // Makes every photon at position collide with most recent photon
            List<Photon> photonsAtPos = recentPhotonDisplacements[pos];

            Photon newPhoton = photonsAtPos[^1];

            for (int i = photonsAtPos.Count - 2; i >= 0; i--)
                if (photonsAtPos[i] != null)
                    HandlePhotonCollision(photonsAtPos[i], newPhoton);
        }

        private void HandlePhotonCollision(Photon photonA, Photon photonB)
        {
            // TODO
            Debug.Log("Found Photon Collision at " + photonA.GetPosition());
        }
        #endregion

        #region Routine
        private IEnumerator ClearDisplacementsCo()
        {
            WaitForEndOfFrame waitForEndOfFrame = new();

            while (true)
            {
                yield return waitForEndOfFrame;
                recentPhotonDisplacements.Clear();
            }
        }
        #endregion
    }
}
