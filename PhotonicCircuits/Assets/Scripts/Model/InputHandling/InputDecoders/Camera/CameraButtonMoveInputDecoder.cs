using Game.Data;
using UnityEngine;

namespace Game
{
    public class CameraButtonMoveInputDecoder : IButtonInputDecoder
    {
        private readonly CameraController camController;

        private Vector2 moveDir;

        public CameraButtonMoveInputDecoder()
        {
            // Cache camera.
            Camera cam = Camera.main;
            camController = cam.GetComponent<CameraController>();

            // Default values.
            moveDir = new();
        }

        #region Decode Input
        public void DecodeInput(InputCode code, ButtonInputType inputType)
        {
            if (inputType == ButtonInputType.Hold)
                return;

            if (!TryDecodeCodeToVector(code, out Vector2 dir))
                return;

            moveDir += inputType == ButtonInputType.Down ? dir : -dir;
            camController.SetMoveDir(moveDir);
        }

        private bool TryDecodeCodeToVector(InputCode code, out Vector2 dir)
        {
            dir = code switch
            {
                InputCode.W => Vector2.up,
                InputCode.A => Vector2.left,
                InputCode.S => Vector2.down,
                InputCode.D => Vector2.right,
                _ => Vector2.zero
            };

            return dir.magnitude > 0f;
        }
        #endregion

        public void Destroy() { }
    }
}
