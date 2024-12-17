using Game.Data;
using SadUtils;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PlayerInputManager : Singleton<PlayerInputManager>
    {
        private Stack<PlayerInputHandler> inputHandlers;

        private List<InputCode> heldButtons;
        private List<MouseInputCode> heldMouseButtons;

        protected override void Awake()
        {
            SetDefaultValues();
            SetInstance(this);
        }

        private void SetDefaultValues()
        {
            inputHandlers = new();
            heldButtons = new();
            heldMouseButtons = new();
        }

        private void Start()
        {
            LoadDefaultInputHandler();
        }

        private void LoadDefaultInputHandler()
        {
            inputHandlers.Push(new GridNeutralInputHandler());
        }

        #region Manage Input Handlers
        public void AddInputHandler(PlayerInputHandler inputHandler)
        {
            if (inputHandlers.Count > 0)
                inputHandlers.Peek().OnDisable();

            inputHandlers.Push(inputHandler);
            inputHandler.OnEnable(heldButtons, heldMouseButtons);
        }

        public void PopInputHandler()
        {
            PlayerInputHandler destroyedHandler = inputHandlers.Pop();
            destroyedHandler.OnDestroy();
        }

        public PlayerInputHandler GetCurrentInputHandler() => inputHandlers.Peek();
        #endregion

        #region Handle Input
        public void HandleMouseButtonInput(MouseInputCode mouseInputCode, ButtonInputType inputType)
        {
            if (inputHandlers.Count == 0)
                return;

            UpdateHeldMouseButton(mouseInputCode, inputType);

            inputHandlers.Peek().HandleMouseButtonInput(mouseInputCode, inputType);
        }

        public void HandleButtonInput(InputCode code, ButtonInputType inputType)
        {
            if (inputHandlers.Count == 0)
                return;

            UpdateHeldButton(code, inputType);

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

        #region Track Held Inputs
        private void UpdateHeldMouseButton(MouseInputCode mouseInputCode, ButtonInputType inputType)
        {
            if (inputType == ButtonInputType.Down)
                heldMouseButtons.Add(mouseInputCode);

            else if (inputType == ButtonInputType.Up && heldMouseButtons.Contains(mouseInputCode))
                heldMouseButtons.Remove(mouseInputCode);
        }

        private void UpdateHeldButton(InputCode code, ButtonInputType inputType)
        {
            if (inputType == ButtonInputType.Down)
                heldButtons.Add(code);

            else if (inputType == ButtonInputType.Up && heldButtons.Contains(code))
                heldButtons.Remove(code);
        }
        #endregion
    }
}
