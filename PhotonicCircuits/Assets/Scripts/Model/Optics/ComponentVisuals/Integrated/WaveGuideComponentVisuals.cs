using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class WaveGuideComponentVisuals : ComponentVisuals
    {
        [SerializeField] protected List<Transform> nodePositions = new List<Transform>();

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
            if(!sourceWaveguide.nodeHandler.HasSetup()) 
                sourceWaveguide.nodeHandler.SetupNodes(nodePositions.Select(t => (Vector2)t.position).ToList(), NodePathIndexesMapper());
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
            List<Vector2> nodePath = sourceWaveguide.nodeHandler.GetNodePath(sourceWaveguide.InPorts[inPortId].position, sourceWaveguide.GetOutPort(inPortId).position);
            photon.ForceMoveAlongNodes(nodePath, sourceWaveguide.nodeHandler);
        }

        public virtual List<List<Vector2>> NodePathIndexesMapper()
        {
            return new List<List<Vector2>> 
            { 
                new List<Vector2> { nodePositions[0].position },
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
