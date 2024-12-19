using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class ICBeamSplitterComponent : WaveGuideComponent
    {
        public override OpticComponentType Type => OpticComponentType.ICBeamSplitter;
        public Action<Photon, Photon> OnSplitPhoton;

        private Dictionary<Photon, ComponentPort> currentPhotons = new Dictionary<Photon, ComponentPort>();
        private Photon firstEnter = null;

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

        public void SetOrientation(Orientation orientation)
        {
            GridManager.Instance.GridController.TryRotateComponentClockwise(this, this.orientation.GetClockwiseIncrementsDiff(orientation));
        }

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
                    ExternalCoroutineExecutionManager.Instance.StartExternalCoroutine(ResolveSplitPhoton(outerpair.Key, outerpair.Value));
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
                            ExternalCoroutineExecutionManager.Instance.StartExternalCoroutine(ResolveInterferePhotons(outerPair, innerPair));
                            photonCount -= 2;
                            isInterfering = true;
                            i--;
                            break;
                        }
                    }
                    if (!isInterfering)
                    {
                        ExternalCoroutineExecutionManager.Instance.StartExternalCoroutine(ResolveContinueMove(outerPair.Key, outerPair.Value));
                        photonCount -= 1;
                        i--;
                    }
                }
            }
            firstEnter = null;
            currentPhotons.Clear();
        }

        private IEnumerator ResolveSplitPhoton(Photon photon, ComponentPort port)
        {
            currentPhotons.Remove(photon);

            float totalWaitTime = GetTotalNodeTravelTime(port, false);
            float splitWaitTime = GetInteractionNodeTimeOffset();

            yield return new WaitForSeconds(splitWaitTime);

            float photonProbabilty = photon.GetAmplitude() / 2;

            Photon topPhoton = photon.Clone();
            Photon bottomPhoton = photon.Clone();
            PhotonManager.Instance.ReplacePhoton(photon, topPhoton, bottomPhoton);

            topPhoton.SetAmplitude(photonProbabilty);
            bottomPhoton.SetAmplitude(photonProbabilty);
            OnSplitPhoton?.Invoke(topPhoton, bottomPhoton);

            yield return new WaitForSeconds(totalWaitTime - splitWaitTime);

            topPhoton.SetPosition(OutPorts[1].position);
            bottomPhoton.SetPosition(OutPorts[2].position);

            topPhoton.TriggerExitComponent(this);
            TriggerOnPhotonExit(topPhoton);
            bottomPhoton.TriggerExitComponent(this);
            TriggerOnPhotonExit(bottomPhoton);
        }

        private IEnumerator ResolveInterferePhotons(KeyValuePair<Photon, ComponentPort> photonA, KeyValuePair<Photon, ComponentPort> photonB)
        {
            float totalWaitTime = GetTotalNodeTravelTime(photonA.Value, false);
            float interfereWaitTime = GetInteractionNodeTimeOffset();

            yield return new WaitForSeconds(totalWaitTime - interfereWaitTime);

            currentPhotons.Remove(photonA.Key);
            currentPhotons.Remove(photonB.Key);

            yield return new WaitForSeconds(interfereWaitTime);

            if (photonA.Key.GetPropagation().IsOnSameAxis(orientation)) (photonA, photonB) = (photonB, photonA);

            PhotonInterferenceManager.Instance.HandleInterference(photonA.Key, photonB.Key, InterferenceType.Split);

            ExitIfExist(photonA.Key);
            ExitIfExist(photonB.Key);
        }

        private IEnumerator ResolveContinueMove(Photon photon, ComponentPort port)
        {
            currentPhotons.Remove(photon);

            yield return new WaitForSeconds(GetTotalNodeTravelTime(port, false));

            photon.SetPosition(GetOutPort(port.portId).position);
            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
        }

        private void ExitIfExist(Photon photon)
        {
            if (PhotonManager.Instance.FindPhoton(photon) == null) return;
            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
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

        private float GetInteractionNodeTimeOffset()
        {
            float totalDistance = Vector2.Distance(InPorts[0].position, pathNodes[0].position);
            float timeToTravelTile = 1f / PhotonMovementManager.Instance.MoveSpeed;
            return (totalDistance * timeToTravelTile);
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

        public override Vector2[] GetNodesByInPortIndex(int inPortIndex)
        {
            return inPortIndex switch
            {
                -1 => new Vector2[] { pathNodes[0].position, pathNodes[2].position, },
                0 => new Vector2[] { pathNodes[0].position, pathNodes[1].position, },
                1 => new Vector2[] { pathNodes[1].position, pathNodes[0].position, },
                2 => new Vector2[] { pathNodes[2].position, pathNodes[0].position, },
                _ => throw new ArgumentException("Invalid inPort")
            };
        }
    }
}
