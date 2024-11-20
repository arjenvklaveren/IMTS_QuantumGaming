using Game.Data;
using System;
using UnityEngine;

namespace Game
{
    public class MirrorComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.Mirror;

        public MirrorComponent(
            Vector2Int[] tilesToOccupy,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts
            ) : base(
                tilesToOccupy,
                inPorts,
                outPorts)
        {
        }

        protected override void HandlePhoton(ComponentPort port, Photon photon)
        {
            int outportID = GetOutPort(port.portId);
            photon.SetPropagation(outPorts[outportID].orientation);

            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
        }

        private int GetOutPort(int inPort)
        {
            return inPort switch
            {
                3 => 2, 2 => 3, 1 => 0, 0 => 1,
                _ => throw new ArgumentException("Invalid inPort")
            };
        }
    }
}
