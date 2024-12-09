using Game.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Game.UI
{
    public class VectorFieldContext : ComponentPropertyContext
    {
        [SerializeField] TMP_InputField inputFieldX;
        [SerializeField] TMP_InputField inputFieldY;
        Vector2 vectorVal;
        bool isVectorInt = false;

        private void Start()
        {
            inputFieldX.onValueChanged.AddListener((value) => OnChangeInputFieldValue(inputFieldX, value, true));
            inputFieldY.onValueChanged.AddListener((value) => OnChangeInputFieldValue(inputFieldY, value, false));
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            if (contextInfo.field.FieldType == typeof(Vector2Int)) isVectorInt = true;
            if (!isVectorInt) vectorVal = (Vector2)contextInfo.field.GetValue(contextInfo.component);
            else vectorVal = (Vector2Int)contextInfo.field.GetValue(contextInfo.component);
            SetInputFieldValues();
        }

        void SetInputFieldValues()
        {
            if (isVectorInt)
            {
                inputFieldX.contentType = TMP_InputField.ContentType.IntegerNumber;
                inputFieldY.contentType = TMP_InputField.ContentType.IntegerNumber;
            }

            inputFieldX.text = vectorVal.x.ToString();
            inputFieldY.text = vectorVal.y.ToString();
        }

        void OnChangeInputFieldValue(TMP_InputField input, string value, bool isX)
        {
            if (string.IsNullOrEmpty(value)) { value = 0.ToString(); input.text = value; return; }

            if (isX) vectorVal.x = float.Parse(value);
            else vectorVal.y = float.Parse(value);

            if (isVectorInt) OnEditValue(GetInvokeParams(contextInfo.onValueChange, Vector2Int.RoundToInt(vectorVal)));
            else contextInfo.field.SetValue(contextInfo.component, vectorVal);
        }
    }
}
