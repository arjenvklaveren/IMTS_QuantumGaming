using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class ICBeamSplitterComponentVisuals : WaveGuideComponentVisuals
    {
        [Header("Object references")]
        [SerializeField] private Transform interfereNode;

        [Header("Prefab references")]
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
            photonVisuals.transform.position = interfereNode.position;

            int outPortId = index == 0 ? 1 : 2;
            List<Vector2> nodePath = sourceWaveguide.nodeHandler.GetNodePath(interfereNode.position, sourceSplitter.OutPorts[outPortId].position);
            photonVisuals.ForceMoveAlongNodes(nodePath, sourceWaveguide.nodeHandler);
        }
        #endregion

        public override List<List<int>> NodePathsIndexesMapper()
        {
            return new List<List<int>>
            {
                new List<int> { 0, 1, 2, 3 },
                new List<int> { 0, 1, 4, 5 }
            };
        }

        protected override void HandlePhotonAlt(PhotonVisuals photon, int inPortId)
        {
            if (photon is PhotonParticleVisuals) photonPrefab = photonParticlePrefab; 
            else photonPrefab = photonBeamPrefab;
            sourceSplitter.SetInferereNode(interfereNode.position);
            List<Vector2> nodePath = sourceWaveguide.nodeHandler.GetNodePath(sourceSplitter.InPorts[inPortId].position, sourceSplitter.GetOutPort(inPortId).position);
            photon.ForceMoveAlongNodes(nodePath, sourceWaveguide.nodeHandler);
        }

        #region Handle Rotation
        protected override void HandleRotationChanged(Orientation orientation)
        {
            RotateToLookAtOrientation(visualsHolder, orientation);
        }
        #endregion 
    }
}
