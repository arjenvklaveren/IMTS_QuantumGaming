using UnityEngine;

namespace Game
{
    public class PhaseShifterComponentVisuals : ComponentVisuals
    {
        protected override void HandlePhoton(PhotonVisuals photon)
        {
            Debug.Log("visuals found photon visuals!");
        }
    }
}
