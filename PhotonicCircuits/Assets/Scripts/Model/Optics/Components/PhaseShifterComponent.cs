using Game.Data;
using UnityEngine;

namespace Game
{
    public class PhaseShifterComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.PhaseShifter;

        public PhaseShifterComponent(
            Vector2Int[] tilesToOccupy,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts
            ) : base(
                tilesToOccupy,
                inPorts,
                outPorts)
        {
        }

        protected override void HandlePhoton(ComponentPort port, Photon photon)
        {
            photon.RotatePhase(Mathf.PI / 2);
            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
        }
    }
}
