using Game.Data;
using UnityEngine;

namespace Game
{
    public interface IPlayerInputHandler
    {
        public void HandleMouseButtonInput(MouseInputCode code, ButtonInputType inputType);
        public void HandleButtonInput(InputCode code, ButtonInputType inputType);
        public void HandleScrollInput(float scrollDelta);
        public void HandleMousePositionDelta(Vector2 mousePositionDelta);

        public void OnDiscard();
    }
}
