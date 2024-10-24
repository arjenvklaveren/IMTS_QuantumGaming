using Game.Data;
using UnityEngine;

namespace Game
{
    public class GridNeutralInputHandler : IPlayerInputHandler
    {
        public void HandleButtonInput(InputCode code, ButtonInputType inputType)
        {
            // Send inputs to camera.
            CameraInputDecoder.DecodeButtonInput(code, inputType);
        }

        public void HandleMouseButtonInput(MouseInputCode code, ButtonInputType inputType)
        {
            // Send inputs to camera.
            CameraInputDecoder.DecodeMouseButtonInput(code, inputType);
        }

        public void HandleMousePositionDelta(Vector2 mousePositionDelta)
        {
            // Send inputs to camera.
            CameraInputDecoder.DecodeMousePositionDeltaInput(mousePositionDelta);
        }

        public void HandleScrollInput(float scrollDelta)
        {
            // Send inputs to camera.
            CameraInputDecoder.DecodeScrollInput(scrollDelta);
        }

        public void OnDiscard()
        {

        }

        private CameraInputDecoder CameraInputDecoder => CameraInputDecoder.Instance;
    }
}
