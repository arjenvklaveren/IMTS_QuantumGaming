using Game.Data;
using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class MirrorComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.Mirror;

        public MirrorComponent(
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
            yield return PhotonMovementManager.Instance.GetWaitMoveTime(photon.GetPhotonType(), true);

            int outportID = GetOutPort(port.portId);
            photon.SetPropagation(OutPorts[outportID].orientation);

            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
        }

        private int GetOutPort(int inPort)
        {
            return inPort += inPort % 2 == 0 ? 1 : -1;
        }

        public override void SetOrientation(Orientation orientation) => ComponentRotateUtil.SetOrientation(this, orientation);
    }
}
