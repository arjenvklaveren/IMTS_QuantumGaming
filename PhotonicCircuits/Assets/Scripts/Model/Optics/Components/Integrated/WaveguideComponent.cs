using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class WaveGuideComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.WaveGuideStraight;
        public Action<int> OnHandlePhoton;

        public WaveguideNodeHandler nodeHandler;
        public float[] nodePathLengths = Array.Empty<float>();

        public WaveGuideComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation defaultOrientation,
            Orientation placeOrientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts,
            float[] nodePathLengths
            ) : base(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                placeOrientation,
                inPorts,
                outPorts)
        {
            nodeHandler = new WaveguideNodeHandler(this);
            this.nodePathLengths = nodePathLengths;
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

        protected void CallExitPhoton(Photon photon, ComponentPort port, bool portIsOutPort = false)
        {
            ExternalCoroutineExecutionManager.Instance.StartExternalCoroutine(CallExitPhotonCo(photon, port, portIsOutPort));
        }
        protected IEnumerator CallExitPhotonCo(Photon photon, ComponentPort port, bool portIsOutPort = false)
        {
            ComponentPort resolvePort = portIsOutPort ? port : GetOutPort(port.portId);

            if(!nodeHandler.HasSetup()) yield return nodeHandler.GetExitWaitTime(photon, nodePathLengths);

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

        public override string SerializeArgs()
        {
            return JsonConvert.SerializeObject(nodePathLengths);
        }

        public override void SetOrientation(Orientation orientation) => ComponentRotateUtil.SetOrientation(this, orientation);
    }
}
