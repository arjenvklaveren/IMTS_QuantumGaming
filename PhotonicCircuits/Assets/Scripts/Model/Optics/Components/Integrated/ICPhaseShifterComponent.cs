using Game.Data;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class ICPhaseShifterComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.ICPhaseShifter;

        public ICPhaseShifterComponent(
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
            yield return PhotonMovementManager.Instance.GetWaitMoveTime(photon.GetPhotonType(), true);

            photon.RotatePhase(Mathf.PI / 2);
            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
        }
    }
}
