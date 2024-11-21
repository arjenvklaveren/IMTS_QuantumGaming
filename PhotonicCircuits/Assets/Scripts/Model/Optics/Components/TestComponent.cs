using Game.Data;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class TestComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.Test;

        public TestComponent(
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
            PhotonManager.Instance.RemovePhoton(photon, false);

            yield break;
        }
    }
}
