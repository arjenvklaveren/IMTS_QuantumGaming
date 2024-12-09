using Game.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class SliderContext : ComponentPropertyContext
    {
        [SerializeField] Slider slider;

        private void Start()
        {
            slider.onValueChanged.AddListener(OnChangeSliderValue);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            SetSliderProperties();
        }

        void SetSliderProperties()
        {
            RangeAttribute rangeAttribute = PropertyContextUtils.FieldContainsRangeAttribute(contextInfo.field);
            slider.minValue = rangeAttribute.min; 
            slider.maxValue = rangeAttribute.max;
            slider.value = (float)contextInfo.field.GetValue(contextInfo.component);
        }

        void OnChangeSliderValue(float value)
        {
            var convertedValue = PropertyContextUtils.ConvertToCorrectType(value, contextInfo.field.FieldType);
            OnEditValue(GetInvokeParams(contextInfo.onValueChange, convertedValue));
        }
    }
}
