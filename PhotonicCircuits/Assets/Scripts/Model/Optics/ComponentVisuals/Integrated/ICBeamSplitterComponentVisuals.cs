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

            List<Vector2> nodes = GetNodesByInPortIndex(nodePath).ToList();
            nodes.RemoveAt(0);
            photonVisuals.transform.position = pathNodes[0].position;
            photonVisuals.ForceMoveAlongNodes(nodes.ToArray(), sourceSplitter.OutPorts[outPort]);
        }
        #endregion

        protected override void HandlePhotonAlt(PhotonVisuals photon, int inPortId)
        {
            sourceWaveguide.SetTotalTravelTime(GetTotalNodeTravelTime(photon, inPortId));
            sourceSplitter.SetInteractionTimeOffset(GetInteractionNodeTimeOffset());
            if (photon is PhotonParticleVisuals) photonPrefab = photonParticlePrefab; 
            else photonPrefab = photonBeamPrefab; 
            photon.ForceMoveAlongNodes(GetNodesByInPortIndex(inPortId), sourceWaveguide.GetOutPort(inPortId));
        }

        public override Vector2[] GetNodesByInPortIndex(int inPortIndex)
        {
            return inPortIndex switch
            {
                -1 => new Vector2[] { pathNodes[0].position, pathNodes[2].position, },
                0 => new Vector2[] { pathNodes[0].position, pathNodes[1].position, },
                1 => new Vector2[] { pathNodes[1].position, pathNodes[0].position, },
                2 => new Vector2[] { pathNodes[2].position, pathNodes[0].position, },
                _ => throw new ArgumentException("Invalid inPort")
            };
        }

        private float GetInteractionNodeTimeOffset()
        {
            float totalDistance = Vector2.Distance(sourceWaveguide.InPorts[0].position, pathNodes[0].position);
            float timeToTravelTile = 1f / PhotonMovementManager.Instance.MoveSpeed;
            return (totalDistance * timeToTravelTile);
        }

        #region Handle Rotation
        protected override void HandleRotationChanged(Orientation orientation)
        {
            RotateToLookAtOrientation(visualsHolder, orientation);
        }
        #endregion 
    }
}
