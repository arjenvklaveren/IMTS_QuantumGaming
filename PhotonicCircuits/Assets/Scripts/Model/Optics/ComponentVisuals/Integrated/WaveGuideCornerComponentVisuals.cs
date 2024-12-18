using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WaveGuideCornerComponentVisuals : WaveGuideComponentVisuals
    { 
        protected override void HandlePhotonAlt(PhotonVisuals photon, int inPortId)
        {
            if (photon is PhotonParticleVisuals)
            {
                PhotonParticleVisuals photonParticle = photon as PhotonParticleVisuals;
                photonParticle.ForceMoveAlongNodes(sourceWaveguide.GetNodesByInPortIndex(inPortId));
            }
            else
            {
                PhotonBeamVisuals photonBeam = photon as PhotonBeamVisuals;
                photonBeam.ForceMoveAlongNodes(sourceWaveguide.GetNodesByInPortIndex(inPortId));
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
