using Game.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class PhotonDetectorComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.Detector;

        [ComponentContext("State identifier", nameof(SetStateIdentifier), true)]
        int stateIdentifier;

        [ComponentContext("Measurements", null, true)]
        int measurements = 0;

        public PhotonDetectorComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation defaultOrientation,
            Orientation placeOrientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts,
            int? stateIdentifier
            ) : base(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                placeOrientation,
                inPorts,
                outPorts)
        {
            SetupListeners();
            if (stateIdentifier == null) SetStateIdentifier(null);
            else this.stateIdentifier = stateIdentifier.Value;
        }

        #region Event handling
        public override void Destroy()
        {
            base.Destroy();
            RemoveListeners();
        }

        void SetupListeners()
        {
            GridController.OnComponentRemoved += GridController_OnRemoveComponent;

        }

        void RemoveListeners()
        {
            GridController.OnComponentRemoved -= GridController_OnRemoveComponent;
        }

        void GridController_OnRemoveComponent(OpticComponent component)
        {
            if (component is PhotonDetectorComponent)
            {
                PhotonDetectorComponent detector = component as PhotonDetectorComponent;
                if (detector.GetStateIdentifier() < stateIdentifier) stateIdentifier -= 1;
            }
        }
        #endregion

        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            yield return PhotonMovementManager.Instance.GetWaitMoveTime(photon.GetPhotonType(), true);

            float photonDetectPercentage = photon.GetAmplitude() * 100;
            float detectComparePercentage = Random.Range(0.0f, 100.0f);

            bool detected = detectComparePercentage <= photonDetectPercentage;
            if (detected)
            {
                MeasuringManager.Instance.MeasurePhoton(this, photon);
                measurements++;
            }
            PhotonManager.Instance.RemovePhoton(photon, detected);

            yield break;
        }

        public void SetStateIdentifier(int? state)
        {
            if (state == null)
            {
                List<PhotonDetectorComponent> detectors =
                    GridManager.Instance.GetActiveGrid().placedComponents.OfType<PhotonDetectorComponent>().ToList();
                stateIdentifier = detectors.Count;
            }
            else stateIdentifier = state.Value;
        }

        public int GetStateIdentifier() { return stateIdentifier; }

        public override string SerializeArgs()
        {
            return stateIdentifier.ToString();
        }

        public override void SetOrientation(Orientation orientation) => ComponentRotateUtil.SetOrientation(this, orientation);
    }
}
