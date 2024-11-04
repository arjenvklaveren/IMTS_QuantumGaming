using Game.Data;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Input
{
    public class PlayerInputReceiver : MonoBehaviour
    {
        [SerializeField] private MouseInputCode[] mouseButtonIds;
        [SerializeField] private KeyCode[] keyCodes;

        private Vector2 lastMousePosition;

        private IEnumerator Start()
        {
            enabled = false;

            yield return PlayerInputManager.WaitForInstance;

            SetDefaultValues();

            enabled = true;
        }

        private void SetDefaultValues()
        {
            lastMousePosition = UnityEngine.Input.mousePosition;
        }

        private void Update()
        {
            ReceiveMouseButtonInputs();

            ReceiveScrollInput();
            ReceiveMousePositionDelta();

            ReceiveButtonInputs();
        }

        #region Receive Inputs
        private void ReceiveMouseButtonInputs()
        {
            foreach (MouseInputCode mouseButtonId in mouseButtonIds)
            {
                if (UnityEngine.Input.GetMouseButtonDown((int)mouseButtonId))
                    PlayerInputManager.HandleMouseButtonInput(mouseButtonId, ButtonInputType.Down);

                else if (UnityEngine.Input.GetMouseButton((int)mouseButtonId))
                    PlayerInputManager.HandleMouseButtonInput(mouseButtonId, ButtonInputType.Hold);

                else if (UnityEngine.Input.GetMouseButtonUp((int)mouseButtonId))
                    PlayerInputManager.HandleMouseButtonInput(mouseButtonId, ButtonInputType.Up);
            }
        }

        private void ReceiveScrollInput()
        {
            float scrollDelta = UnityEngine.Input.mouseScrollDelta.y;

            if (Mathf.Abs(scrollDelta) > 0f)
                PlayerInputManager.HandleScrollInput(scrollDelta);
        }

        private void ReceiveMousePositionDelta()
        {
            Vector2 currentMousePos = UnityEngine.Input.mousePosition;

            Vector2 delta = currentMousePos - lastMousePosition;
            if (delta.magnitude > 0f)
                PlayerInputManager.HandleMousePositionDelta(delta);

            lastMousePosition = currentMousePos;
        }

        private void ReceiveButtonInputs()
        {
            foreach (KeyCode keyCode in keyCodes)
            {
                if (!TryParseKeyCodeToInputCode(keyCode, out InputCode inputCode))
                    continue;

                if (UnityEngine.Input.GetKeyDown(keyCode))
                    PlayerInputManager.HandleButtonInput(inputCode, ButtonInputType.Down);

                else if (UnityEngine.Input.GetKey(keyCode))
                    PlayerInputManager.HandleButtonInput(inputCode, ButtonInputType.Hold);

                else if (UnityEngine.Input.GetKeyUp(keyCode))
                    PlayerInputManager.HandleButtonInput(inputCode, ButtonInputType.Up);
            }
        }

        private bool TryParseKeyCodeToInputCode(KeyCode code, out InputCode inputCode)
        {
            string codeString = code.ToString();

            return Enum.TryParse(codeString, out inputCode);
        }
        #endregion

        private PlayerInputManager PlayerInputManager => PlayerInputManager.Instance;
    }
}
