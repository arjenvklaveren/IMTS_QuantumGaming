using Game.Data;

namespace Game
{
    public class ICOutComponentVisuals : ComponentVisuals
    {
        protected override void HandlePhoton(PhotonVisuals photon)
        {
            if (photon.source.GetPhotonType() != PhotonType.Quantum)
                return;

            Destroy(photon.gameObject);
        }

        protected override void HandleRotationChanged(Orientation orientation)
        {
            RotateToLookAtOrientation(visualsHolder, orientation);
        }
    }
}
