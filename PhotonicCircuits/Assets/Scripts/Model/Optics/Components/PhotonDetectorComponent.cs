using Game.Data;
using UnityEngine;

namespace Game
{
    public class PhotonDetectorComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.Detector;

        public PhotonDetectorComponent(
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
            PhotonManager.Instance.RemovePhoton(photon, true);
        }
    }
}
