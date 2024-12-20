using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WaveGuideComponentVisuals : ComponentVisuals
    {
        [SerializeField] protected List<Transform> pathNodes = new List<Transform>();

        protected WaveGuideComponent sourceWaveguide;
        protected PhotonVisuals visuals;

        [Header("Visuals Holder")]
        [SerializeField] protected Transform visualsHolder;

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
            sourceWaveguide.SetTotalTravelTime(GetTotalNodeTravelTime(photon, inPortId));
            if (photon is PhotonParticleVisuals)
            {
                PhotonParticleVisuals photonParticle = photon as PhotonParticleVisuals;
                photonParticle.ForceMoveAlongNodes(GetNodesByInPortIndex(inPortId));
            }
            else
            {
                PhotonBeamVisuals photonBeam = photon as PhotonBeamVisuals;

            }
        }

        protected float GetTotalNodeTravelTime(PhotonVisuals photon, int inPortIndex, bool includeHalfTileOffset = true)
        { 
            ComponentPort inPort = sourceWaveguide.InPorts[inPortIndex];
            Vector2[] nodes = GetNodesByInPortIndex(inPortIndex);
            float totalDistance = Vector2.Distance(inPort.position, nodes[0]);
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                totalDistance += Vector2.Distance(nodes[i], nodes[i + 1]);
            }
            float timeToTravelTile = (photon.source.GetPhotonType() == PhotonType.Quantum) 
                ? 1f / PhotonMovementManager.Instance.MoveSpeed :
                1f / (PhotonMovementManager.Instance.MoveSpeed * PhotonMovementManager.Instance.ClassicSpeedMultiplier);

            totalDistance += Vector2.Distance(sourceWaveguide.GetOutPort(inPort.portId).position, nodes[nodes.Length - 1]);
            float outTimeVal = (totalDistance * timeToTravelTile);
            if (includeHalfTileOffset) outTimeVal += (timeToTravelTile / 2);
            return outTimeVal;
        }

        public virtual Vector2[] GetNodesByInPortIndex(int inPortIndex)
        {
            return inPortIndex switch
            {
                0 => new Vector2[] { pathNodes[0].position },
                1 => new Vector2[] { pathNodes[0].position },
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
