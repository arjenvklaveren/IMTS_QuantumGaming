using Game.Data;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class PhaseShifterComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.PhaseShifter;

        [ComponentContext("Phase shift", "SetShiftValue"), Range(0.0f,Mathf.PI * 2)]
        private float shiftValueRadians;
        [ComponentContext("Offset", "SetImperfectOffsetValue")] 
        private float imperfectOffsetValue = 50;
        [ComponentContext("Toggle test", "SetTogglePhaseShift")] 
        private bool toggleTest;

        public PhaseShifterComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts,
            float shiftValueRadians
            ) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts)
        {
            this.shiftValueRadians = shiftValueRadians;
        }

        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            yield return PhotonMovementManager.Instance.WaitForMoveHalfTile;

            photon.RotatePhase(shiftValueRadians);
            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);
        }

        #region Value changes
        public void SetTogglePhaseShift(bool toggle)
        {
            toggleTest = toggle;
        }
        public void SetShiftValue(float value)
        {
            shiftValueRadians = value;
        }
        public void SetImperfectOffsetValue(float value)
        {
            imperfectOffsetValue = value;
        }
        #endregion

        public override string SerializeArgs()
        {
            return shiftValueRadians.ToString();
        }
    }
}
