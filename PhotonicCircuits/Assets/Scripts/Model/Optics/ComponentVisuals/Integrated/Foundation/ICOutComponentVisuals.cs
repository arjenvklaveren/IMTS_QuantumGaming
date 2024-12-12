using Game.Data;
using UnityEngine;

namespace Game
{
    public class ICOutComponentVisuals : ComponentVisuals
    {
        [SerializeField] private Transform visualsHolder;

        protected override void HandlePhoton(PhotonVisuals photon)
        {
            Destroy(photon.gameObject);
        }

        protected override void HandleRotationChanged(Orientation orientation)
        {
            RotateToLookAtOrientation(visualsHolder, orientation);
        }
    }
}
