using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Game
{
    public class WaveGuideComponentVisuals : ComponentVisuals
    {
        [SerializeField] protected List<Transform> pathNodes = new List<Transform>();

        protected WaveGuideComponent sourceWaveguide;
        protected PhotonVisuals visuals;

        #region Awake/Destroy
        public override void SetSource(OpticComponent component)
        {
            base.SetSource(component);
            SetDefaultValues();
            SetupListeners();
        }
        protected virtual void SetDefaultValues()
        {
            sourceWaveguide = SourceComponent as WaveGuideComponent;
            sourceWaveguide.SetPathNodesCopy(pathNodes);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveListeners();
        }

        private void SetupListeners()
        {
            sourceWaveguide.OnHandlePhoton += Source_OnHandlePhoton;
        }
        private void RemoveListeners()
        {
            sourceWaveguide.OnHandlePhoton -= Source_OnHandlePhoton;
        }

        protected void Source_OnHandlePhoton(int portId) => HandlePhotonAlt(visuals, portId);
        #endregion

        protected override void HandlePhoton(PhotonVisuals photon) { visuals = photon; }
        protected virtual void HandlePhotonAlt(PhotonVisuals photon, int inPortId)
        {
            if (photon is PhotonParticleVisuals)
            {
                PhotonParticleVisuals photonParticle = photon as PhotonParticleVisuals;
                photonParticle.ForceMoveAlongNodes(sourceWaveguide.GetNodesByInPortIndex(inPortId));
            }
            else
            {
                PhotonBeamVisuals photonBeam = photon as PhotonBeamVisuals;

            }
        }
    }
}
