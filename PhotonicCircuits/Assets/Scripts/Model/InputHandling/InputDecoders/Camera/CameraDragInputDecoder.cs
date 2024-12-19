using Game.Data;
using UnityEngine;

namespace Game
{
    public class CameraDragInputDecoder : IMouseButtonInputDecoder, IMousePositionDeltaInputDecoder
    {
        private readonly CameraController camController;

        private bool isDragging;

        public CameraDragInputDecoder()
        {
            // Cache camera.
            Camera cam = Camera.main;
            camController = cam.GetComponent<CameraController>();
        }

        public void DecodeInput(MouseInputCode code, ButtonInputType inputType)
        {
            if (code != MouseInputCode.LeftMouseButton)
                return;

            if (inputType == ButtonInputType.Hold)
                return;

            isDragging = inputType == ButtonInputType.Down;
        }

        public void DecodeInput(Vector2 mousePositionDelta)
        {
            if (!isDragging)
                return;

            camController.Drag(mousePositionDelta);
        }

        public void OnDisable()
        {
            isDragging = false;
        }

        public void Destroy() { }
    }
}
