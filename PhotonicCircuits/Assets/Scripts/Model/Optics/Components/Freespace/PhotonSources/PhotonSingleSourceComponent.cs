using Game.Data;
using System;
using UnityEngine;

namespace Game
{
    public class PhotonSingleSourceComponent : PhotonSourceComponent
    {
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
            
        }

        #region Create Photons
        protected override void CreatePhoton()
        {
            ComponentPort spawnPort = OutPorts[0];

            Photon photon = new(
                HostGrid,
                spawnPort.position,
                spawnPort.orientation);

            photon.SetUniqueSourceKey(GetUniqueSourceKey());

            PhotonManager.Instance.AddPhoton(photon);
            OnCreatePhoton?.Invoke(photon);

            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
        }
        #endregion
    }
}
