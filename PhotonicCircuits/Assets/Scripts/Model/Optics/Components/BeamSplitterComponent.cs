using Game.Data;
using System;
using UnityEngine;

namespace Game
{
    public class BeamSplitterComponent : OpticComponent
    {
        public event Action<Photon, Photon> OnSplitPhoton;

        public override OpticComponentType Type => OpticComponentType.BeamSplitter;

        public BeamSplitterComponent(
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
            SplitPhoton(photon, port.portId);
            PhotonManager.Instance.RemovePhoton(photon, false);
        }

        private void SplitPhoton(Photon photon, int inPortIndex)
        {
            int[] outPortIndexes = GetOutPorts(inPortIndex);
            ComponentPort reflectOutPort = outPorts[outPortIndexes[0]];
            ComponentPort passOutPort = outPorts[outPortIndexes[1]];

            Photon passPhoton = photon.Clone();
            Photon reflectPhoton = photon.Clone();
            PhotonManager.Instance.ReplacePhoton(photon, passPhoton, reflectPhoton);
            reflectPhoton.SetPropagation(reflectOutPort.orientation);
            OnSplitPhoton?.Invoke(passPhoton, reflectPhoton);

            passPhoton.TriggerExitComponent(this);
            TriggerOnPhotonExit(passPhoton);
            reflectPhoton.TriggerExitComponent(this);
            TriggerOnPhotonExit(reflectPhoton);
        }

        private int[] GetOutPorts(int inPort)
        {
            return inPort switch
            {
                3 => new int[] { 2, 1 }, 
                2 => new int[] { 3, 0 },
                1 => new int[] { 0, 3 },
                0 => new int[] { 1, 2 },
                _ => throw new ArgumentException("Invalid inPort")
            };
        }
    }
}
