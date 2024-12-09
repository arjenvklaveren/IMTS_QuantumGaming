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
            float photonDetectPercentage = photon.GetAmplitude() * 100;
            float detectComparePercentage = Random.Range(0.0f, 100.0f);

            //Debug.Log("PHOTON DETECTED AT: " + occupiedRootTile);

            if(detectComparePercentage <= photonDetectPercentage)
            {
                PhotonManager.Instance.RemovePhoton(photon, true);
            }
            else
            {
                PhotonManager.Instance.RemovePhoton(photon, false);
            }

            yield break;
        }
    }
}
