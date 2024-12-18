using Game.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ICPhaseShifterComponentVisuals : ComponentVisuals
    {
        [Header("Visuals Holder")]
        [SerializeField] private Transform visualsHolder;

        protected override void HandlePhoton(PhotonVisuals photon)
        {
            if (photon is PhotonParticleVisuals)
            {
                PhotonParticleVisuals photonParticle = photon as PhotonParticleVisuals;

                // Force move photon visuals to center of component.
                Vector2 photonStartPos = photon.transform.position;
                Vector2 photonEndPos = GridUtils.GridPos2WorldPos(SourceComponent.occupiedRootTile, SourceComponent.HostGrid);

                photonParticle.ForceMoveHalfTile(photonStartPos, photonEndPos);
            }
            else
            {
                PhotonBeamVisuals photonBeam = photon as PhotonBeamVisuals;

            }
        }

        #region Handle Rotation
        protected override void HandleRotationChanged(Orientation orientation)
        {
            RotateToLookAtOrientation(visualsHolder, orientation);
        }
        #endregion 
    }
}
