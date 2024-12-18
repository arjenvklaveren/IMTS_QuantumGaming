using Game.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class ICBeamSplitterComponentVisuals : WaveGuideComponentVisuals
    {
        [SerializeField] private PhotonParticleVisuals photonParticlePrefab;
        [SerializeField] private PhotonBeamVisuals photonBeamPrefab;

        private PhotonVisuals photonPrefab;
        private ICBeamSplitterComponent sourceSplitter;

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
            sourceSplitter = SourceComponent as ICBeamSplitterComponent;
        }

        private void SetupListeners()
        {
            sourceSplitter.OnSplitPhoton += Source_OnSplitPhoton;
        }

        private void RemoveListeners()
        {
            sourceSplitter.OnSplitPhoton -= Source_OnSplitPhoton;
        }
        #endregion

        #region Handle Events
        private void Source_OnSplitPhoton(Photon photon1, Photon photon2) => HandlePhotonCreation(photon1, photon2);

        private void HandlePhotonCreation(params Photon[] photons)
        {
            for (int i = 0; i < photons.Length; i++)
            {
                CreatePhotonVisuals(photons[i], i);
            }
        }

        private void CreatePhotonVisuals(Photon photon, int index)
        {
            PhotonVisuals photonVisuals = Instantiate(photonPrefab);
            photonVisuals.SetSource(photon);
            photonVisuals.SetAsInComponent(sourceSplitter);

            int outPort = index == 0 ? 1 : 2;
            int nodePath = outPort == 1 ? 0 : -1;

            List<Vector2> nodes = sourceWaveguide.GetNodesByInPortIndex(nodePath).ToList();
            nodes.RemoveAt(0);
            photonVisuals.transform.position = pathNodes[0].position;
            photonVisuals.ForceMoveAlongNodes(nodes.ToArray(), sourceSplitter.OutPorts[outPort]);
        }
        #endregion

        protected override void HandlePhotonAlt(PhotonVisuals photon, int inPortId)
        {
            if (photon is PhotonParticleVisuals) photonPrefab = photonParticlePrefab; 
            else photonPrefab = photonBeamPrefab; 
            photon.ForceMoveAlongNodes(sourceWaveguide.GetNodesByInPortIndex(inPortId), sourceWaveguide.GetOutPort(inPortId));
        }

        #region Handle Rotation
        protected override void HandleRotationChanged(Orientation orientation)
        {
            RotateToLookAtOrientation(visualsHolder, orientation);
        }
        #endregion 
    }
}
