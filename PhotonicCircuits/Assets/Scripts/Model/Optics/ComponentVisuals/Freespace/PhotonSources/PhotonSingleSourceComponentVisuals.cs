using Game.Data;
using SadUtils;
using UnityEngine;

namespace Game
{
    public class PhotonSingleSourceComponentVisuals : PhotonSourceComponentVisuals
    {      
        protected override void HandlePhotonCreation(Photon photon)
        {
            PhotonVisuals photonVisuals = Instantiate(photonPrefab);
            photonVisuals.SetSource(photon);
            photonVisuals.SyncVisuals();
            photonVisuals.StartMovement();
        }
    }
}
