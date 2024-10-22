using SadUtils;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Input
{
    public class PlayerInputManager : Singleton<PlayerInputManager>
    {
        private Stack<IPlayerInputHandler> inputHandlers;

        #region Awake
        protected override void Awake()
        {
            SetInstance(this);

            inputHandlers = new();
            SetupListeners();
        }

        private void SetupListeners()
        {
            PlayerInputReceiver.OnLMBDown += PlayerInputReceiver_OnLMBDown;
            PlayerInputReceiver.OnLMBUp += PlayerInputReceiver_OnLMBUp;

            PlayerInputReceiver.OnRMBDown += PlayerInputReceiver_OnRMBDown;
            PlayerInputReceiver.OnRMBUp += PlayerInputReceiver_OnRMBUp;

            PlayerInputReceiver.OnScroll += PlayerInputReceiver_OnScroll;

            PlayerInputReceiver.OnMouseDelta += PlayerInputReceiver_OnMouseDelta;
        }
        #endregion

        #region OnDestroy
        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void RemoveListeners()
        {
            PlayerInputReceiver.OnLMBDown -= PlayerInputReceiver_OnLMBDown;
            PlayerInputReceiver.OnLMBUp -= PlayerInputReceiver_OnLMBUp;

            PlayerInputReceiver.OnRMBDown -= PlayerInputReceiver_OnRMBDown;
            PlayerInputReceiver.OnRMBUp -= PlayerInputReceiver_OnRMBUp;

            PlayerInputReceiver.OnScroll -= PlayerInputReceiver_OnScroll;

            PlayerInputReceiver.OnMouseDelta -= PlayerInputReceiver_OnMouseDelta;
        }
        #endregion

        #region Input Handling
        private void PlayerInputReceiver_OnLMBDown()
        {
            if (inputHandlers.Count == 0)
                return;

            inputHandlers.Peek().HandleLMBDown();
        }

        private void PlayerInputReceiver_OnLMBUp()
        {
            if (inputHandlers.Count == 0)
                return;

            inputHandlers.Peek().HandleLMBUp();
        }

        private void PlayerInputReceiver_OnRMBDown()
        {
            if (inputHandlers.Count == 0)
                return;

            inputHandlers.Peek().HandleRMBDown();
        }

        private void PlayerInputReceiver_OnRMBUp()
        {
            if (inputHandlers.Count == 0)
                return;

            inputHandlers.Peek().HandleRMBUp();
        }

        private void PlayerInputReceiver_OnScroll(float delta)
        {
            if (inputHandlers.Count == 0)
                return;

            inputHandlers.Peek().HandleScroll(delta);
        }

        private void PlayerInputReceiver_OnMouseDelta(Vector2 delta)
        {
            if (inputHandlers.Count == 0)
                return;

            inputHandlers.Peek().HandleMouseDelta(delta);
        }
        #endregion

        #region Manager Input Handler
        public void AddInputHandler(IPlayerInputHandler inputHandler)
        {
            inputHandlers.Push(inputHandler);
        }

        public void RemoveInputHandler()
        {
            inputHandlers.Pop();
        }
        #endregion
    }
}
