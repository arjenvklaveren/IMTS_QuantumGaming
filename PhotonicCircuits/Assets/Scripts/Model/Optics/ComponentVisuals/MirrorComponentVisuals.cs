using UnityEngine;

namespace Game
{
    public class MirrorComponentVisuals : ComponentVisuals
    {
        protected override void HandlePhoton(PhotonVisuals photon)
        {
            Debug.Log("visuals found photon visuals!");
        }
    }
}
