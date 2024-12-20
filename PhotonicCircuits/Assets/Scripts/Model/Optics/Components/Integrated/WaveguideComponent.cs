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
        public Action<int> OnHandlePhoton;
        protected float totalNodeTravelTime;

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

        public void SetTotalTravelTime(float totalTravelTime) { this.totalNodeTravelTime = totalTravelTime; }

        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            OnHandlePhoton?.Invoke(port.portId);
            yield return new WaitForSeconds(totalNodeTravelTime);

            photon.SetPosition(GetOutPort(port.portId).position);
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
