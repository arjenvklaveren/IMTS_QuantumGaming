using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class CameraZoomInputDecoder : IScrollInputDecoder
    {
        private readonly CameraController camController;

        public CameraZoomInputDecoder()
        {
            Camera cam = Camera.main;
            camController = cam.GetComponent<CameraController>();
        }

        public void DecodeInput(float scrollDelta)
        {
            if (!CanScroll())
                return;

            // Inverse scroll delta.
            camController.UpdateZoom(-scrollDelta);
        }

        private bool CanScroll()
        {
            return GridTile.IsHovered || ComponentVisuals.IsHovered || !EventSystem.current.IsPointerOverGameObject();
        }

        public void OnDisable() { }
        public void Destroy() { }
    }
}
