using UnityEngine;

namespace Game
{
    public class CameraZoomInputDecoder : IScrollInputDecoder
    {
        private CameraController camController;

        public CameraZoomInputDecoder()
        {
            Camera cam = Camera.main;
            camController = cam.GetComponent<CameraController>();
        }

        public void DecodeInput(float scrollDelta)
        {
            // Inverse scroll delta.
            camController.UpdateZoom(-scrollDelta);
        }

        public void Destroy() { }
    }
}
