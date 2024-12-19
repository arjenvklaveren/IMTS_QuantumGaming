using Game.Data;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class PhaseShifterComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.PhaseShifter;

        [ComponentContext("Phase shift", "SetShiftValue"), Range(0.0f, Mathf.PI * 2)]
        private float shiftValueRadians;
        [ComponentContext("Phase offset", "SetOffsetValue"), Range(-Mathf.PI / 2, Mathf.PI / 2)]
        private float offsetValue = 0;

        public PhaseShifterComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation defaultOrientation,
            Orientation placeOrientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts,
            float shiftValueRadians
            ) : base(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                placeOrientation,
                inPorts,
                outPorts)
        {
            this.shiftValueRadians = shiftValueRadians;
        }

        public void SetOrientation(Orientation orientation)
        {
            GridManager.Instance.GridController.TryRotateComponentClockwise(this, this.orientation.GetClockwiseIncrementsDiff(orientation));
        }

        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            yield return PhotonMovementManager.Instance.GetWaitMoveTime(photon.GetPhotonType(), true);

            Debug.Log(shiftValueRadians);
            Debug.Log(offsetValue);

            photon.RotatePhase(shiftValueRadians + offsetValue);
            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
        }

        #region Value changes
        public void SetShiftValue(float value)
        {
            shiftValueRadians = value;
        }
        public void SetOffsetValue(float value)
        {
            offsetValue = value;
        }
        #endregion

        public override string SerializeArgs()
        {
            return shiftValueRadians.ToString();
        }
    }
}
