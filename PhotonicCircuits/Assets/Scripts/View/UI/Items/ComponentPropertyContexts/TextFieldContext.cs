using Game.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class TextFieldContext : ComponentPropertyContext
    {
        [SerializeField] TMP_InputField inputField;

        private void Start()
        {
            inputField.onValueChanged.AddListener(OnChangeInputFieldValue);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            SetInputFieldValues();
        }

        void SetInputFieldValues()
        {
            inputField.text = contextInfo.field.GetValue(contextInfo.component).ToString();
        }

        void OnChangeInputFieldValue(string value)
        {
            OnEditValue(GetInvokeParams(contextInfo.onValueChange, value));
        }
    }
}
