using Game.Data;
using SadUtils;
using UnityEngine;

namespace Game
{
    public class CameraInputDecoder : Singleton<CameraInputDecoder>
    {
        private bool[] directionalInputsPressed;

        private bool isDragging;

        protected override void Awake()
        {
            SetDefaultValues();

            SetInstance(this);
        }

        private void SetDefaultValues()
        {
            directionalInputsPressed = new bool[4];
        }

        #region WASD Movement Inputs
        public void DecodeButtonInput(InputCode code, ButtonInputType inputType)
        {
            if (inputType == ButtonInputType.Hold)
                return;

            int directionalId = GetDirectionIdFromCode(code);

            if (directionalId < 0)
                return;

            bool state = inputType == ButtonInputType.Down;
            directionalInputsPressed[directionalId] = state;
        }

        private int GetDirectionIdFromCode(InputCode code)
        {
            return code switch
            {
                InputCode.W => 0,
                InputCode.A => 1,
                InputCode.S => 2,
                InputCode.D => 3,
                _ => -1
            };
        }
        #endregion

        #region Drag Toggle Inputs
        public void DecodeMouseButtonInput(MouseInputCode code, ButtonInputType inputType)
        {

        }
        #endregion

        #region Draw Inputs
        public void DecodeMousePositionDeltaInput(Vector2 mousePositionDelta)
        {

        }
        #endregion

        #region Zoom Inputs
        public void DecodeScrollInput(float scrollDelta)
        {

        }
        #endregion

        #region Read Inputs
        #region Read Move Vector
        public Vector2 GetMoveVector()
        {

        }
        #endregion
        #endregion
    }
}
