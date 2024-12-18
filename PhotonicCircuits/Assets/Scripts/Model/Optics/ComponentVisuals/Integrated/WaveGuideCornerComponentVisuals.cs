using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WaveGuideCornerComponentVisuals : WaveGuideComponentVisuals
    {
        [SerializeField] GameObject cornerBase;
        [SerializeField] GameObject cornerAlt;

        private WaveGuideCornerComponent sourceCorner;

        #region Awake / Destroy
        public override void SetSource(OpticComponent component)
        {
            base.SetSource(component);
            SetDefaultValues();
            SetupListeners();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveListeners();
        }

        protected override void SetDefaultValues()
        {
            base.SetDefaultValues();
            sourceCorner = SourceComponent as WaveGuideCornerComponent;
        }

        private void SetupListeners()
        {
            sourceCorner.OnChangeAltType += Source_OnChangeAltType;
        }

        private void RemoveListeners()
        {
            sourceCorner.OnChangeAltType -= Source_OnChangeAltType;
        }
        #endregion

        private void Source_OnChangeAltType(bool isAlt)
        {
            cornerBase.SetActive(!isAlt);
            cornerAlt.SetActive(isAlt);
        }


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
