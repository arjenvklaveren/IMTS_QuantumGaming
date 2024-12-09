using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ToggleContext : ComponentPropertyContext
    {
        [SerializeField] private Toggle toggle;

        private void Start()
        {
            toggle.onValueChanged.AddListener(OnToggle);
        }

        protected override void OnInitialize()
        {
            toggle.isOn = (bool)contextInfo.field.GetValue(contextInfo.component);
        }

        void OnToggle(bool isOn)
        {
            OnEditValue(GetInvokeParams(contextInfo.onValueChange, isOn));
        }
    }
}
