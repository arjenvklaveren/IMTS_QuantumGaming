using Game.Data;
using System;
using UnityEngine;

namespace Game
{
    public class PhotonSingleSourceComponent : OpticComponent
    {
        public event Action<Photon> OnCreatePhoton;

        public override OpticComponentType Type => OpticComponentType.SourceSingle;

        public PhotonSingleSourceComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation defaultOrientation,
            Orientation placeOrientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts
            ) : base(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                placeOrientation,
                inPorts,
                outPorts)
        {
            SetupListeners();
        }

        public void SetOrientation(Orientation orientation)
        {
            GridManager.Instance.GridController.TryRotateComponentClockwise(this, this.orientation.GetClockwiseIncrementsDiff(orientation));
        }

        #region Handle Events
        private void SetupListeners()
        {
            SimulationManager.OnSimulationStart += SimulationManager_OnSimulationStart;
        }

        public override void Destroy()
        {
            SimulationManager.OnSimulationStart -= SimulationManager_OnSimulationStart;
        }

        private void SimulationManager_OnSimulationStart()
        {
            CreatePhoton();
        }
        #endregion

        #region Create Photons
        private void CreatePhoton()
        {
            ComponentPort spawnPort = OutPorts[0];

            Photon photon = new(
                HostGrid,
                spawnPort.position,
                spawnPort.orientation);

            PhotonManager.Instance.AddPhoton(photon);
            OnCreatePhoton?.Invoke(photon);

            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
        }
        #endregion
    }
}
