using Game.Data;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class PhaseShifterComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.PhaseShifter;
        private float shiftValueRadians;

        public PhaseShifterComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts,
            float shiftValueRadians
            ) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts)
        {
            this.shiftValueRadians = shiftValueRadians;
        }

        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            yield return PhotonMovementManager.Instance.WaitForMoveHalfTile;

            photon.RotatePhase(0);
            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
        }

        public override string SerializeArgs()
        {
            return shiftValueRadians.ToString();
        }
    }
}
