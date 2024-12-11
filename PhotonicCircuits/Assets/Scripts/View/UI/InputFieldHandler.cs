using System;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class InputFieldHandler : MonoBehaviour
    {
        public Action<string> OnEndEdit;

        [SerializeField] private TMP_InputField inputField;
        public TMP_Text placeholder;

        private void Awake()
        {
            inputField.onEndEdit.AddListener(HandleEndEdit);
        }

        private void OnDestroy()
        {
            inputField.onEndEdit.RemoveListener(HandleEndEdit);
        }

        private void HandleEndEdit(string input)
        {
            OnEndEdit?.Invoke(input);
        }

        public void SetAsActiveField()
        {
            inputField.ActivateInputField();
        }
    }
}
