using Game.Data;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class PhotonDetectorComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.Detector;

        public PhotonDetectorComponent(
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
            PhotonManager.Instance.RemovePhoton(photon, true);
            yield break;
        }
    }
}
