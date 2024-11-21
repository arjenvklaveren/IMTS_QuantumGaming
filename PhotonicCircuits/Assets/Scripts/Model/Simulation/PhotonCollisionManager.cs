using Game.Data;
using SadUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PhotonCollisionManager : Singleton<PhotonCollisionManager>
    {
        private readonly struct PhotonDisplacementData
        {
            public readonly Photon photon;

            public readonly Orientation Orientation => photon.GetPropagation();
            public readonly float recordedTime;

            public PhotonDisplacementData(
                Photon photon)
            {
                this.photon = photon;
                recordedTime = Time.time;
            }
        }

        private Dictionary<GridData, Dictionary<Vector2Int, List<Photon>>> recentPhotonDisplacements;
        private Dictionary<GridData, Dictionary<Vector2Int, List<PhotonDisplacementData>>> flyingPhotons;

        private Dictionary<int, Coroutine> collisionDelayRoutines;
        private Coroutine clearRoutine;

        private int collisionCounter;

        private float timeToTravelTile;

        #region Awake / Destroy
        protected override void Awake()
        {
            SetInstance(this);

            recentPhotonDisplacements = new();
            flyingPhotons = new();
            collisionDelayRoutines = new();
            collisionCounter = 0;
        }

        private IEnumerator Start()
        {
            yield return PhotonMovementManager.WaitForInstance;
            SetupListeners();

            timeToTravelTile = 1f / PhotonMovementManager.Instance.MoveSpeed;
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void SetupListeners()
        {
            PhotonMovementManager.Instance.OnPhotonDisplace += PhotonMovementManger_OnPhotonDisplace;
            OpticComponent.OnPhotonExit += OpticComponent_OnPhotonExit;
            SimulationManager.OnSimulationInitialize += SimulationManager_OnSimulationInitialize;
            SimulationManager.OnSimulationStop += SimulationManager_OnSimulationStop;
        }

        private void RemoveListeners()
        {
            PhotonMovementManager.Instance.OnPhotonDisplace -= PhotonMovementManger_OnPhotonDisplace;
            OpticComponent.OnPhotonExit -= OpticComponent_OnPhotonExit;
            SimulationManager.OnSimulationInitialize -= SimulationManager_OnSimulationInitialize;
            SimulationManager.OnSimulationStop -= SimulationManager_OnSimulationStop;
        }
        #endregion

        #region Handle Events
        private void PhotonMovementManger_OnPhotonDisplace(Photon photon, bool isFinal) => RegisterPhotonDisplacement(photon, isFinal);
        private void OpticComponent_OnPhotonExit(Photon photon) => HandleOpposingPhotonCollision(photon);

        private void SimulationManager_OnSimulationInitialize()
        {
            clearRoutine = StartCoroutine(ClearDisplacementsCo());
        }

        private void SimulationManager_OnSimulationStop()
        {
            StopCoroutine(clearRoutine);

            foreach (Coroutine routine in collisionDelayRoutines.Values)
                StopCoroutine(routine);

            recentPhotonDisplacements.Clear();
            flyingPhotons.Clear();
            collisionDelayRoutines.Clear();
        }
        #endregion

        #region Handle Photons
        private void RegisterPhotonDisplacement(Photon photon, bool isFinal)
        {
            if (!isFinal)
                HandleNonOpposingPhotonCollision(photon);

            HandleOpposingPhotonCollision(photon, isFinal);
        }

        #region Non Opposing Photons
        private void HandleNonOpposingPhotonCollision(Photon photon)
        {
            Vector2Int photonPos = photon.GetPosition();

            // Only handle non opposing outside of occupied tiles
            if (photon.currentGrid.occupiedTiles.Contains(photonPos))
                return;

            List<Photon> displacementsEntry = GetRecentDisplacementEntry(photon, photonPos);

            displacementsEntry.Add(photon);

            if (displacementsEntry.Count > 2)
                HandlePhotonCollisionsAtPosition(displacementsEntry);
        }

        private List<Photon> GetRecentDisplacementEntry(in Photon photon, in Vector2Int photonPos)
        {
            if (!recentPhotonDisplacements.ContainsKey(photon.currentGrid))
                recentPhotonDisplacements.Add(photon.currentGrid, new());

            if (!recentPhotonDisplacements[photon.currentGrid].ContainsKey(photonPos))
                recentPhotonDisplacements[photon.currentGrid].Add(photonPos, new());

            return recentPhotonDisplacements[photon.currentGrid][photonPos];
        }

        private void HandlePhotonCollisionsAtPosition(List<Photon> photonsAtPos)
        {
            Photon newPhoton = photonsAtPos[^1];
            Orientation opposingOrientation = newPhoton.GetPropagation().RotateClockwise(2);

            for (int i = photonsAtPos.Count - 2; i >= 0; i--)
                if (ShouldCollide(photonsAtPos[i], opposingOrientation))
                    HandlePhotonCollision(photonsAtPos[i], newPhoton);
        }

        private bool ShouldCollide(Photon photonAtPos, Orientation opposingOrientation)
        {
            if (photonAtPos == null)
                return false;

            // Only allow collision between non opposing photons
            return photonAtPos.GetPropagation() != opposingOrientation;
        }
        #endregion

        #region Opposing Photons
        private void HandleOpposingPhotonCollision(Photon photon, bool isFinal = false)
        {
            RemoveLastRecord(photon);

            if (isFinal)
                return;

            if (DoesNextPosContainOpposingPhoton(photon, out List<PhotonDisplacementData> opposingPhotonData))
                HandleOpposingPhotonDataCollisions(opposingPhotonData, photon);

            RegisterFlyingPhoton(photon);
        }

        private void RemoveLastRecord(Photon photon)
        {
            Vector2Int previousPosition = photon.GetPosition() - photon.GetPropagationIntVector();

            // Detect if last record exists
            if (!DoesRecordExist(photon, previousPosition, out List<PhotonDisplacementData> record))
                return;

            // Search for last record and remove
            foreach (PhotonDisplacementData data in record)
            {
                if (data.photon != photon)
                    continue;

                record.Remove(data);
                return;
            }
        }

        private bool DoesRecordExist(Photon photon, Vector2Int position, out List<PhotonDisplacementData> record)
        {
            record = new();

            if (!flyingPhotons.ContainsKey(photon.currentGrid))
                return false;

            if (!flyingPhotons[photon.currentGrid].ContainsKey(position))
                return false;

            record = flyingPhotons[photon.currentGrid][position];
            return true;
        }

        private bool DoesNextPosContainOpposingPhoton(Photon photon, out List<PhotonDisplacementData> opposingPhotonData)
        {
            opposingPhotonData = new();
            Vector2Int nextPos = photon.GetPosition() + photon.GetPropagationIntVector();

            if (!DoesRecordExist(photon, nextPos, out List<PhotonDisplacementData> record))
                return false;

            Orientation opposingOrientation = photon.GetPropagation().RotateClockwise(2);

            foreach (PhotonDisplacementData data in record)
                if (data.Orientation == opposingOrientation)
                    opposingPhotonData.Add(data);

            return opposingPhotonData.Count > 0;
        }

        private void RegisterFlyingPhoton(Photon photon)
        {
            Vector2Int position = photon.GetPosition();

            if (!flyingPhotons.ContainsKey(photon.currentGrid))
                flyingPhotons.Add(photon.currentGrid, new());

            if (!flyingPhotons[photon.currentGrid].ContainsKey(position))
                flyingPhotons[photon.currentGrid].Add(position, new());

            flyingPhotons[photon.currentGrid][position].Add(new(photon));
        }

        private void HandleOpposingPhotonDataCollisions(List<PhotonDisplacementData> opposingPhotonData, Photon collidingPhoton)
        {
            foreach (PhotonDisplacementData data in opposingPhotonData)
                HandleOpposingPhotonDataCollision(data, collidingPhoton);
        }

        private void HandleOpposingPhotonDataCollision(PhotonDisplacementData opposingPhotonData, Photon collidingPhoton)
        {
            float timeDif = Time.time - opposingPhotonData.recordedTime;

            float timeLeft = timeToTravelTile - timeDif;
            float timeToCollision = timeLeft / 2f;

            Coroutine delayCollisionRoutine = StartCoroutine(
                CollidePhotonsAfterDelayCo(
                    opposingPhotonData.photon,
                    collidingPhoton,
                    timeToCollision,
                    collisionCounter));

            collisionDelayRoutines.Add(collisionCounter, delayCollisionRoutine);
            collisionCounter++;
        }
        #endregion
        #endregion

        private void HandlePhotonCollision(Photon photonA, Photon photonB)
        {
            // TODO
            Debug.Log("Found Photon Collision at " + photonA.GetPosition());
        }

        #region Routines
        private IEnumerator ClearDisplacementsCo()
        {
            WaitForEndOfFrame waitForEndOfFrame = new();

            while (true)
            {
                yield return waitForEndOfFrame;
                recentPhotonDisplacements.Clear();
            }
        }

        private IEnumerator CollidePhotonsAfterDelayCo(Photon photonA, Photon photonB, float delay, int id)
        {
            yield return new WaitForSeconds(delay);
            HandlePhotonCollision(photonA, photonB);

            collisionDelayRoutines.Remove(id);
        }
        #endregion
    }
}
