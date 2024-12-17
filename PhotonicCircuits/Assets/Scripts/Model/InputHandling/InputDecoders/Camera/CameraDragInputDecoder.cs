using Game.Data;
using UnityEngine;

namespace Game
{
    public class CameraDragInputDecoder : IMouseButtonInputDecoder, IMousePositionDeltaInputDecoder
    {
        public void DecodeInput(MouseInputCode code, ButtonInputType inputType)
        {

        }

        public void DecodeInput(Vector2 mousePositionDelta)
        {

        }

        public void OnDisable() { }
        public void Destroy() { }
    }
}
