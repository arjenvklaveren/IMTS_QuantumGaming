using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;
using Game.Data;
using System;

namespace Game.UI
{
    public struct ContextObjectInfo
    {
        public ComponentContextAttribute attribute;
        public FieldInfo field;
        public OpticComponent component;
        public MethodInfo onValueChange;
    }
    public abstract class ComponentPropertyContext : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI attributeText;
        public ContextObjectInfo contextInfo;

        public void SetInitialValues(ContextObjectInfo contextInfo)
        {
            this.contextInfo = contextInfo;
            if (contextInfo.attribute.attributeName != null) attributeText.text = contextInfo.attribute.attributeName; 
            OnInitialize();
        }
        protected virtual void OnInitialize() { }

        protected void OnEditValue(object[] invokeParams)
        {
            contextInfo.onValueChange.Invoke(contextInfo.component, invokeParams);
        }
        protected object[] GetInvokeParams(MethodInfo methodInfo, object inVal)
        {
            ParameterInfo[] paramInfo =  methodInfo.GetParameters();
            int paramCount = paramInfo.Length;
            object[] outParams = new object[paramCount];
            outParams[0] = inVal;
            for (int i = 1; i < paramCount; i++) outParams[i] = null;
            return outParams;
        }
    }
}
