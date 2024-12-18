using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Data;

namespace Game
{
    public class ICPhaseShifterComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.ICPhaseShifter;

        public ICPhaseShifterComponent(
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

            photon.RotatePhase(Mathf.PI / 2);
            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
        }
    }
}
