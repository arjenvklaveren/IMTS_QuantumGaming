using Game.Data;
using UnityEngine;

namespace Game
{
    public class TestComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.Test;

        public TestComponent(
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts
            ) : base(
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts)
        {
        }

        protected override void HandlePhoton(ComponentPort port, Photon photon)
        {
            PhotonManager.Instance.RemovePhoton(photon, false);
        }
    }
}
