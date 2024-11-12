using Game.Data;
using UnityEngine;

namespace Game
{
    public class TestComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.Test;

        public TestComponent(
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
            Debug.Log($"Port {port.portId} detected Photon!");
            Debug.Log("Destroy Photon Here!");
        }
    }
}
