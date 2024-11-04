using UnityEngine;

namespace Game
{
    public interface IMousePositionDeltaInputDecoder : IInputDecoder
    {
        public void DecodeInput(Vector2 mousePositionDelta);
    }
}
