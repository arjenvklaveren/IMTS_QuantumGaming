using Game.Data;
using UnityEngine;

namespace Game
{
    public class MirrorComponentVisuals : ComponentVisuals
    {
        protected override void HandlePhoton(PhotonVisuals photon)
        {
            Vector2 photonStartPos = photon.transform.position;
            Vector2 photonEndPos = GridUtils.GridPos2WorldPos(SourceComponent.occupiedRootTile, SourceComponent.HostGrid);

            photon.ForceMoveHalfTile(photonStartPos, photonEndPos);
        }
    }
}
