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
            Orientation orientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts
            ) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts)
        {
            SetupListeners();
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
            ComponentPort spawnPort = outPorts[0];

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
