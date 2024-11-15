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
            Debug.Log("IN: " + inPortIndex + " | " + "OUT: " + outPortIndexes[0]);
            ComponentPort reflectOutPort = outPorts[outPortIndexes[0]];
            ComponentPort passOutPort = outPorts[outPortIndexes[1]];

            Photon passPhoton = photon.Clone();
            Photon reflectPhoton = photon.Clone();
            PhotonManager.Instance.ReplacePhoton(photon, passPhoton, reflectPhoton);
            reflectPhoton.SetPropagation(reflectOutPort.orientation);
            Debug.Log(reflectPhoton.GetPropagation());

            Debug.Log(PhotonManager.Instance.GetAllPhotons().Count);

            OnSplitPhoton?.Invoke(passPhoton, reflectPhoton);
        }

        public int[] GetOutPorts(int inPort)
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
