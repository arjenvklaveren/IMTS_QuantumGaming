using Game.Data;
using SadUtils;
using UnityEngine;

namespace Game
{
    public class PhotonLaserSourceComponentVisuals : PhotonSourceComponentVisuals
    {
        protected override void HandlePhotonCreation(Photon photon)
        {
            PhotonVisuals photonVisuals = Instantiate(photonPrefab);
            photonVisuals.transform.position = transform.position;
            photonVisuals.SetSource(photon);
        }
    }
}
