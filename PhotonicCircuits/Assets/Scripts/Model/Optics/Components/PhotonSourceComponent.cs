using Game.Data;
using System;
using UnityEngine;

namespace Game
{
    public class PhotonSourceComponent : OpticComponent
    {
        public event Action<Photon> OnCreatePhoton;

        public override OpticComponentType Type => OpticComponentType.Source;

        public PhotonSourceComponent(
            Vector2Int[] tilesToOccupy,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts
            ) : base(
                tilesToOccupy,
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
