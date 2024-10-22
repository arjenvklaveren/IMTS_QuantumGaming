using System;
using System.Collections;
using UnityEngine;

namespace Game.Input
{
    public class PlayerInputReceiver : MonoBehaviour
    {
        public static event Action OnLMBDown;
        public static event Action OnLMBUp;

        public static event Action OnRMBDown;
        public static event Action OnRMBUp;

        public static event Action<float> OnScroll;
        public static event Action<Vector2> OnMouseDelta;

        private Vector2 lastMousePosition;

        private IEnumerator Start()
        {
            gameObject.SetActive(false);

            yield return PlayerInputManager.WaitForInstance;

            gameObject.SetActive(true);

            // Set default values
            lastMousePosition = UnityEngine.Input.mousePosition;
        }

        private void Update()
        {
            ReceiveLMBInputs();
            ReceiveRMBInputs();

            ReceiveScrollInput();

            ReceiveMouseDelta();
        }

        private void ReceiveLMBInputs()
        {
            if (UnityEngine.Input.GetMouseButtonDown(0))
                OnLMBDown?.Invoke();

            if (UnityEngine.Input.GetMouseButtonUp(0))
                OnLMBUp?.Invoke();
        }

        private void ReceiveRMBInputs()
        {
            if (UnityEngine.Input.GetMouseButtonDown(1))
                OnLMBDown?.Invoke();

            if (UnityEngine.Input.GetMouseButtonUp(1))
                OnLMBUp?.Invoke();
        }

        private void ReceiveScrollInput()
        {
            float scrollDelta = UnityEngine.Input.mouseScrollDelta.y;

            if (Mathf.Abs(scrollDelta) > 0)
                OnScroll?.Invoke(scrollDelta);
        }

        private void ReceiveMouseDelta()
        {
            Vector2 mousePosition = UnityEngine.Input.mousePosition;

            Vector2 delta = mousePosition - lastMousePosition;

            if (delta.magnitude > 0f)
                OnMouseDelta?.Invoke(delta);

            lastMousePosition = mousePosition;
        }
    }
}
