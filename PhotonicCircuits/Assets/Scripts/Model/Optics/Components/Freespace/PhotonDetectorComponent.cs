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

        public override void SetOrientation(Orientation orientation)
        {
            GridManager.Instance.GridController.TryRotateComponentClockwise(this, this.orientation.GetIncrementsDiff(orientation));
        }

        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            yield return PhotonMovementManager.Instance.GetWaitMoveTime(photon.GetPhotonType(), true);

            float photonDetectPercentage = photon.GetAmplitude() * 100;
            float detectComparePercentage = Random.Range(0.0f, 100.0f);

            bool detected = detectComparePercentage <= photonDetectPercentage;
            PhotonManager.Instance.RemovePhoton(photon, detected);
            if (detected) MeasuringManager.Instance.MeasurePhoton(this, photon);

            yield break;
        }
    }
}
