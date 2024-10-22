using UnityEngine;

namespace Game
{
    public interface IPlayerInputHandler
    {
        public void HandleLMBDown();
        public void HandleLMBUp();

        public void HandleRMBDown();
        public void HandleRMBUp();

        public void HandleScroll(float delta);

        public void HandleMouseDelta(Vector2 delta);
    }
}
