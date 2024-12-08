using Game.Data;
using SadUtils;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlayerInputManager : Singleton<PlayerInputManager>
    {
        private Stack<PlayerInputHandler> inputHandlers;

        protected override void Awake()
        {
            inputHandlers = new();
            LoadDefaultInputHandler();

            SetInstance(this);
        }

        private void LoadDefaultInputHandler()
        {
            inputHandlers.Push(new GridComponentPaintInputHandler());
        }

        #region Manage Input Handlers
        public void AddInputHandler(PlayerInputHandler inputHandler)
        {
            inputHandlers.Push(inputHandler);
        }

        public void PopInputHandler()
        {
            PlayerInputHandler destroyedHandler = inputHandlers.Pop();
            destroyedHandler.OnDestroy();
        }
        #endregion

        #region Handle Input
        public void HandleMouseButtonInput(MouseInputCode mouseInputCode, ButtonInputType inputType)
        {
            if (inputHandlers.Count == 0)
                return;

            inputHandlers.Peek().HandleMouseButtonInput(mouseInputCode, inputType);
        }

        public void HandleButtonInput(InputCode code, ButtonInputType inputType)
        {
            if (inputHandlers.Count == 0)
                return;

            inputHandlers.Peek().HandleButtonInput(code, inputType);
        }

        public void HandleScrollInput(float scrollDelta)
        {
            if (inputHandlers.Count == 0)
                return;

            inputHandlers.Peek().HandleScrollInput(scrollDelta);
        }

        public void HandleMousePositionDelta(Vector2 mousePositionDelta)
        {
            if (inputHandlers.Count == 0)
                return;

            inputHandlers.Peek().HandleMousePositionDelta(mousePositionDelta);
        }
        #endregion
    }
}
