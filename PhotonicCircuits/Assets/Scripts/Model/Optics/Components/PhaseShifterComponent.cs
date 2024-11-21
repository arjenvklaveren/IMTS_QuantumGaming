using Game.Data;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class PhaseShifterComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.PhaseShifter;

        public PhaseShifterComponent(
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

        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            photon.RotatePhase(Mathf.PI / 2);
            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);

            yield break;
        }
    }
}
