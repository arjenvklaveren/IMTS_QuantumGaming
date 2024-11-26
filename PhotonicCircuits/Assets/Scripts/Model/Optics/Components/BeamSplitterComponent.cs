using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game
{
    public class BeamSplitterComponent : OpticComponent
    {
        public event Action<Photon, Photon> OnSplitPhoton;

        public override OpticComponentType Type => OpticComponentType.BeamSplitter;
        private Dictionary<Photon, ComponentPort> currentPhotons = new Dictionary<Photon, ComponentPort>();
        private Photon firstEnter = null;

        public BeamSplitterComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts
            ) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts)
        {
            
        }

        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            currentPhotons.Add(photon, port);
            if (firstEnter == null) HandleFirstPhoton(photon);

            yield break;
        }

        private void HandleFirstPhoton(Photon photon)
        {
            Debug.Log("FIRST PHOTON HAS ENTERED");
            firstEnter = photon;
            firstEnter.OnExitComponent += HandleAllCurrentPhotonsCo;
        }

        private void HandleAllCurrentPhotonsCo(OpticComponent component)
        {
            Debug.Log("FIRST PHOTON HAS LEFT");
            firstEnter.OnExitComponent -= HandleAllCurrentPhotonsCo;
            firstEnter = null;
        }

        private void ResolveSplitPhoton(Photon photon, ComponentPort inPort)
        {
            int[] outPortIndexes = GetOutPorts(inPort);
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

        private void ResolveInterferePhoton()
        {
            
        }

        private bool IsInterference()
        {
            if (currentPhotons.Count > 1)
            {

            }
            return false;
        }

        private int[] GetOutPorts(ComponentPort inPort)
        {
            int portIndex = inPort.portId;
            return portIndex switch
            {
                3 => new int[] { 2, 1 },
                2 => new int[] { 3, 0 },
                1 => new int[] { 0, 3 },
                0 => new int[] { 1, 2 },
                _ => throw new ArgumentException("Invalid inPort")
            };
        }
        private bool IsInterferePort(ComponentPort inPort, ComponentPort otherPort)
        {
            int inIndex = inPort.portId, otherIndex = otherPort.portId;
            return inIndex switch
            {
                3 => (otherIndex == 0),
                2 => (otherIndex == 1),
                1 => (otherIndex == 2),
                0 => (otherIndex == 3),
                _ => throw new ArgumentException("Invalid inPort")
            };
        }
    }
}
