using UnityEngine;

namespace Game
{
    public class TestComponentVisuals : ComponentVisuals
    {
        protected override void HandlePhoton(PhotonVisuals photon)
        {
            Debug.Log("visuals found photon visuals!");
        }
    }
}
