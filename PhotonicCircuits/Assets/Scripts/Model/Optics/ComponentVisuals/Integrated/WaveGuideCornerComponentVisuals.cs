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
            sourceWaveguide.SetTotalTravelTime(GetTotalNodeTravelTime(photon, inPortId));
            if (photon is PhotonParticleVisuals)
            {
                PhotonParticleVisuals photonParticle = photon as PhotonParticleVisuals;
                photonParticle.ForceMoveAlongNodes(GetNodesByInPortIndex(inPortId));
            }
            else
            {
                PhotonBeamVisuals photonBeam = photon as PhotonBeamVisuals;
                photonBeam.ForceMoveAlongNodes(GetNodesByInPortIndex(inPortId));
            }
        }

        public override Vector2[] GetNodesByInPortIndex(int inPortIndex)
        {
            return inPortIndex switch
            {
                0 => new Vector2[] { pathNodes[0].position, pathNodes[1].position, },
                1 => new Vector2[] { pathNodes[1].position, pathNodes[0].position, },
                _ => throw new ArgumentException("Invalid inPort")
            };
        }

        #region Handle Rotation
        protected override void HandleRotationChanged(Orientation orientation)
        {
            RotateToLookAtOrientation(visualsHolder, orientation);
        }
        #endregion 
    }
}
