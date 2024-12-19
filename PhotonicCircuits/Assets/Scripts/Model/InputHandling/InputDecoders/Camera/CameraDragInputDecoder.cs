using Game.Data;
using UnityEngine;
using UnityEngine.EventSystems;

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

            switch (inputType)
            {
                case ButtonInputType.Up:
                    isDragging = false;
                    break;

                case ButtonInputType.Down:
                    isDragging = CanStartDrag();
                    break;
            }
        }

        public void DecodeInput(Vector2 mousePositionDelta)
        {
            if (!isDragging)
                return;

            camController.Drag(mousePositionDelta);
        }

        private bool CanStartDrag()
        {
            return GridTile.IsHovered || ComponentVisuals.IsHovered || !EventSystem.current.IsPointerOverGameObject();
        }

        public void OnDisable()
        {
            isDragging = false;
        }

        public void Destroy() { }
    }
}
