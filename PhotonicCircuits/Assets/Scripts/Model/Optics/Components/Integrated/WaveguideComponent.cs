using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WaveGuideComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.WaveGuideStraight;
        protected List<Transform> pathNodes = new List<Transform>();
        public Action<int> OnHandlePhoton;

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

        public void SetPathNodesCopy(List<Transform> pathNodes) { this.pathNodes = pathNodes; }

        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            OnHandlePhoton?.Invoke(port.portId);
            yield return new WaitForSeconds(GetTotalNodeTravelTime(port));

            photon.SetPosition(GetOutPort(port.portId).position);
            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
        }

        protected float GetTotalNodeTravelTime(ComponentPort inPort, bool includeHalfTileOffset = true)
        {
            int inPortIndex = inPort.portId;
            Vector2[] nodes = GetNodesByInPortIndex(inPortIndex);
            float totalDistance = Vector2.Distance(inPort.position, nodes[0]);
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                totalDistance += Vector2.Distance(nodes[i], nodes[i + 1]);
            }
            float timeToTravelTile = 1f / PhotonMovementManager.Instance.MoveSpeed;
            totalDistance += Vector2.Distance(GetOutPort(inPort.portId).position, nodes[nodes.Length - 1]);
            float outTimeVal = (totalDistance * timeToTravelTile);
            if (includeHalfTileOffset) outTimeVal += (timeToTravelTile / 2);
            return outTimeVal;
        }

        public virtual Vector2[] GetNodesByInPortIndex(int inPortIndex)
        {
            return inPortIndex switch
            {
                0 => new Vector2[] { new Vector2(0,0) + occupiedRootTile },
                1 => new Vector2[] { new Vector2(0,0) + occupiedRootTile },
                _ => throw new ArgumentException("Invalid inPort")
            };
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
