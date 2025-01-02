using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game
{
    public class ICBeamSplitterComponent : WaveGuideComponent
    {
        public override OpticComponentType Type => OpticComponentType.ICBeamSplitter;

        public Action<Photon, Photon> OnSplitPhoton;
        private Dictionary<Photon, ComponentPort> currentPhotons = new Dictionary<Photon, ComponentPort>();
        private Photon firstEnter = null;
        private Vector2 interfereNode;

        public ICBeamSplitterComponent(
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

        public override void SetOrientation(Orientation orientation) => ComponentRotateUtil.SetOrientation(this, orientation);
        public void SetInferereNode(Vector2 node) { interfereNode = node; }

        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            OnHandlePhoton?.Invoke(port.portId);
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
            for (int i = 0; i < photonCount; i++)
            {
                KeyValuePair<Photon, ComponentPort> outerpair = currentPhotons.ElementAt(i);
                if (outerpair.Value.portId == 0)
                {
                    SetInterfereAction(() => ResolveSplitPhoton(outerpair.Key), outerpair);
                }
                else
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
                            SetInterfereAction(() => ResolveInterferePhotons(outerPair, innerPair), outerPair, innerPair);
                            photonCount -= 2;
                            isInterfering = true;
                            i--;
                            break;
                        }
                    }
                    if (!isInterfering)
                    {
                        SetInterfereAction(() => ResolveContinueMove(outerPair.Key, outerPair.Value), outerPair);
                        photonCount -= 1;
                        i--;
                    }
                }
            }
            firstEnter = null;
            currentPhotons.Clear();
        }

        private void SetInterfereAction(UnityAction action, KeyValuePair<Photon, ComponentPort> photon1, KeyValuePair<Photon, ComponentPort>? photon2 = null)
        {
            nodeHandler.AddNodeAction(new NodeAction(interfereNode, photon1.Key, action));
            if(photon2.HasValue) nodeHandler.AddNodeAction(new NodeAction(interfereNode, photon2.Value.Key, action));  
        }

        private void ResolveSplitPhoton(Photon photon)
        {
            currentPhotons.Remove(photon);

            float photonProbabilty = photon.GetAmplitude() / 2;

            Photon topPhoton = photon.Clone();
            Photon bottomPhoton = photon.Clone();
            PhotonManager.Instance.ReplacePhoton(photon, topPhoton, bottomPhoton);

            topPhoton.SetAmplitude(photonProbabilty);
            bottomPhoton.SetAmplitude(photonProbabilty);
            OnSplitPhoton?.Invoke(topPhoton, bottomPhoton);

            nodeHandler.AddNodeAction(new NodeAction(OutPorts[1].position, topPhoton, () => CallExitPhoton(topPhoton, OutPorts[1], true)));
            nodeHandler.AddNodeAction(new NodeAction(OutPorts[2].position, bottomPhoton, () => CallExitPhoton(bottomPhoton, OutPorts[2], true)));
        }

        private void ResolveInterferePhotons(KeyValuePair<Photon, ComponentPort> photonA, KeyValuePair<Photon, ComponentPort> photonB)
        {
            currentPhotons.Remove(photonA.Key);
            currentPhotons.Remove(photonB.Key);

            if (photonA.Key.GetPropagation().IsOnSameAxis(orientation)) (photonA, photonB) = (photonB, photonA);

            PhotonInterferenceManager.Instance.HandleInterference(photonA.Key, photonB.Key, InterferenceType.Split);

            ComponentPort outPort = GetOutPort(photonA.Value.portId);
            nodeHandler.AddNodeAction(new NodeAction(outPort.position, photonA.Key, () => ExitIfExists(photonA.Key, outPort, true)));
            nodeHandler.AddNodeAction(new NodeAction(outPort.position, photonB.Key, () => ExitIfExists(photonB.Key, outPort, true)));
        }

        private void ResolveContinueMove(Photon photon, ComponentPort port)
        {
            currentPhotons.Remove(photon);
            ComponentPort outPort = GetOutPort(port.portId);
            nodeHandler.AddNodeAction(new NodeAction(outPort.position, photon, () => CallExitPhoton(photon, outPort, true)));
        }

        private void ExitIfExists(Photon photon, ComponentPort port, bool portIsOutPort = false)
        {
            if (PhotonManager.Instance.FindPhoton(photon) == null) return;
            CallExitPhoton(photon, port, portIsOutPort);
        }

        private bool IsInterferePort(ComponentPort inPort, ComponentPort otherPort)
        {
            int inIndex = inPort.portId, otherIndex = otherPort.portId;
            return inIndex switch
            {
                0 => false,
                1 => (otherIndex == 2),
                2 => (otherIndex == 1),
                _ => throw new ArgumentException("Invalid inPort")
            };
        }

        public override ComponentPort GetOutPort(int inPortIndex)
        {
            return inPortIndex switch
            {
                0 => OutPorts[1],
                1 => OutPorts[0],
                2 => OutPorts[0],
                _ => throw new ArgumentException("Invalid inPort")
            };
        }
    }
}
