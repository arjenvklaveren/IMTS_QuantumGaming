using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class WaveGuideComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.WaveGuideStraight;
        public Action<int> OnHandlePhoton;

        public WaveguideNodeHandler nodeHandler = new WaveguideNodeHandler();

        public WaveGuideComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation defaultOrientation,
            Orientation placeOrientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts
            ) : base(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                placeOrientation,
                inPorts,
                outPorts)
        {
            
        }

        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            OnHandlePhoton?.Invoke(port.portId);
            NodeAction endNodeAction = new NodeAction(GetOutPort(port.portId).position, photon,() => 
            {
                photon.SetPropagation(GetOutPort(port.portId).orientation); 
                CallExitPhoton(photon, port);
            });

            nodeHandler.AddNodeAction(endNodeAction);
            yield break;
        }

        public virtual void CallExitPhoton(Photon photon, ComponentPort port, bool portIsOutPort = false)
        {
            ComponentPort resolvePort = portIsOutPort ? port : GetOutPort(port.portId);
            photon.SetPosition(resolvePort.position);
            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
        }

        public virtual ComponentPort GetOutPort(int inPortIndex)
        {
            return inPortIndex switch
            {
                0 => OutPorts[1],
                1 => OutPorts[0],
                _ => throw new ArgumentException("Invalid inPort")
            };
        }

        public override void SetOrientation(Orientation orientation) => ComponentRotateUtil.SetOrientation(this, orientation);
    }
}
