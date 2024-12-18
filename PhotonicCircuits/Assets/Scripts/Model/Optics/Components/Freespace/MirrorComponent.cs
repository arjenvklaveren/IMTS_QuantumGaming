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

        public override void SetOrientation(Orientation orientation)
        {
            GridManager.Instance.GridController.TryRotateComponentClockwise(this, this.orientation.GetIncrementsDiff(orientation));
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
            return inPort switch
            {
                3 => 2,
                2 => 3,
                1 => 0,
                0 => 1,
                _ => throw new ArgumentException("Invalid inPort")
            };

            // could look like this:
            // return inPort += inPort % 2 == 0 ? 1 : -1;
        }
    }
}
