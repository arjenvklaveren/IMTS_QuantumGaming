using Game.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

namespace Game.UI
{
    public class NumberFieldContext : ComponentPropertyContext
    {
        [SerializeField] TMP_InputField inputField;
        [SerializeField] Image inputFieldImage;
        [SerializeField] TextMeshProUGUI inputFieldText;

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
            if (contextInfo.attribute.uneditable)
            {
                inputFieldImage.enabled = false;
                inputFieldText.color = Color.white;
                inputField.interactable = false;
            }
        }

        void OnChangeInputFieldValue(string value)
        {
            if (string.IsNullOrEmpty(value)) { value = 0.ToString(); inputField.text = value; return; }
            var convertedValue = PropertyContextUtils.ConvertToCorrectType(value, contextInfo.field.FieldType);
            OnEditValue(GetInvokeParams(contextInfo.onValueChange, convertedValue));
        }
    }
}
