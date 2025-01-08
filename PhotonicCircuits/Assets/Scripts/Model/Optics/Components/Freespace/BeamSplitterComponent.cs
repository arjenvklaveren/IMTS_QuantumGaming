using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            if (currentPhotons.ContainsKey(photon)) yield break;

            currentPhotons.Add(photon, port);
            if (firstEnter == null)
                ExternalCoroutineExecutionManager.Instance.StartExternalCoroutine(HandleFirstPhoton(photon));
            yield break;
        }

        private IEnumerator HandleFirstPhoton(Photon photon)
        {
            firstEnter = photon;
            yield return PhotonMovementManager.Instance.GetWaitMoveTime(photon.GetPhotonType(), true);
            HandleAllCurrentPhotons();
        }

        private void HandleAllCurrentPhotons()
        {
            int photonCount = currentPhotons.Count;
            if (photonCount == 1)
            {
                KeyValuePair<Photon, ComponentPort> singlePair = currentPhotons.ElementAt(0);
                ResolveSplitPhoton(singlePair.Key, singlePair.Value);
                photonCount = 0;
            }
            for (int i = 0; i < photonCount; i++)
            {
                bool isInterfering = false;
                KeyValuePair<Photon, ComponentPort> outerPair = currentPhotons.ElementAt(i);
                for (int j = 0; j < photonCount; j++)
                {
                    KeyValuePair<Photon, ComponentPort> innerPair = currentPhotons.ElementAt(j);
                    if (i == j) continue;
                    if (PhotonInterferenceManager.Instance.IsInterfering(outerPair.Key, innerPair.Key, false) &&
                        IsInterferePort(outerPair.Value, innerPair.Value))
                    {
                        ResolveInterferePhotons(outerPair, innerPair);
                        isInterfering = true;
                        photonCount -= 2;
                        i--;
                        break;
                    }
                }
                if (!isInterfering)
                {
                    ResolveSplitPhoton(outerPair.Key, outerPair.Value);
                    photonCount -= 1;
                    i--;
                }
            }
            firstEnter = null;
            currentPhotons.Clear();
        }

        private void ResolveSplitPhoton(Photon photon, ComponentPort inPort)
        {
            currentPhotons.Remove(photon);

            int[] outPortIndexes = GetOutPorts(inPort);
            ComponentPort reflectOutPort = OutPorts[outPortIndexes[0]];
            ComponentPort passOutPort = OutPorts[outPortIndexes[1]];

            float photonProbabilty = photon.GetAmplitude() / 2;

            Photon passPhoton = photon.Clone();
            Photon reflectPhoton = photon.Clone();
            PhotonManager.Instance.ReplacePhoton(photon, passPhoton, reflectPhoton);

            reflectPhoton.SetPropagation(reflectOutPort.orientation);
            passPhoton.SetAmplitude(photonProbabilty);
            reflectPhoton.SetAmplitude(photonProbabilty);
            OnSplitPhoton?.Invoke(passPhoton, reflectPhoton);

            passPhoton.TriggerExitComponent(this);
            TriggerOnPhotonExit(passPhoton);
            reflectPhoton.TriggerExitComponent(this);
            TriggerOnPhotonExit(reflectPhoton);
        }

        private void ResolveInterferePhotons(KeyValuePair<Photon, ComponentPort> photonA, KeyValuePair<Photon, ComponentPort> photonB)
        {
            currentPhotons.Remove(photonA.Key);
            currentPhotons.Remove(photonB.Key);

            if (photonA.Key.GetPropagation().IsOnSameAxis(orientation)) (photonA, photonB) = (photonB, photonA);

            PhotonInterferenceManager.Instance.HandleInterference(photonA.Key, photonB.Key, InterferenceType.Split);

            ExitIfExist(photonA.Key);
            ExitIfExist(photonB.Key);
        }

        private void ExitIfExist(Photon photon)
        {
            if (PhotonManager.Instance.FindPhoton(photon) == null) return;
            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
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

        public override void SetOrientation(Orientation orientation) => ComponentRotateUtil.SetOrientation(this, orientation);
    }
}
