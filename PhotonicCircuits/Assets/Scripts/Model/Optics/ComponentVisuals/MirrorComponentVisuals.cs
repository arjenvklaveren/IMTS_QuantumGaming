using Game.Data;
using UnityEngine;

namespace Game
{
    public class MirrorComponentVisuals : ComponentVisuals
    {
        [SerializeField] private Transform visualsHolder;

        protected override void HandlePhoton(PhotonVisuals photon)
        {
            Vector2 photonStartPos = photon.transform.position;
            Vector2 photonEndPos = GridUtils.GridPos2WorldPos(SourceComponent.occupiedRootTile, SourceComponent.HostGrid);

            photon.ForceMoveHalfTile(photonStartPos, photonEndPos);
        }

        protected override void HandleRotationChanged(Orientation orientation) => RotateToLookAtOrientation(visualsHolder, orientation);
    }
}
