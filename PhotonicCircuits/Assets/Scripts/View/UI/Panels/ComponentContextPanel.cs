using Game.Data;
using Game.Model;
using System.Reflection;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

namespace Game.UI
{
    public class ComponentContextPanel : Panel
    {
        [Header("Object references")]
        [SerializeField] GameObject contextPropertiesContainer;
        [SerializeField] ComponentPropertyLookup prefabLookup;

        List<ComponentPropertyContext> contexts = new List<ComponentPropertyContext>();

        OpticComponent selectedComponent;
        bool toggleDisableBackground = false;

        ComponentSelectionManager selectionManager;

        private void Start()
        {
            selectionManager = ComponentSelectionManager.Instance;
            SetupListeners();
        }

        void SetupListeners()
        {
            selectionManager.OnSelectedComponent += ComponentSelectionManager_OnSelectComponent;
        }

        private void ComponentSelectionManager_OnSelectComponent(ComponentVisuals component)
        {
            selectedComponent = selectionManager.SelectedVisuals.SourceComponent;
            ClearContextItems();
            ReflectComponentAttributes();
        }

        void ReflectComponentAttributes()
        {       
            FieldInfo[] fields = PropertyContextUtils.GetContextAttributes(selectedComponent);
            foreach (FieldInfo field in fields) CreateContextItem(field);
        }

        void CreateContextItem(FieldInfo field)
        {
            ComponentPropertyType propertyType = ConvertAttributeToPropertyType(field);
            ComponentPropertyContext context = prefabLookup.GetPrefabByComponentType(propertyType);

            ContextObjectInfo contextObjectInfo = new ContextObjectInfo();
            contextObjectInfo.attribute = field.GetCustomAttribute<ComponentContextAttribute>();
            contextObjectInfo.field = field;
            contextObjectInfo.component = selectedComponent;
            if(contextObjectInfo.attribute.changeValueMethodName != null)
            {
                contextObjectInfo.onValueChange = contextObjectInfo.component.GetType().GetMethod(contextObjectInfo.attribute.changeValueMethodName);
                if(contextObjectInfo.onValueChange != null)
                {
                    if (contextObjectInfo.onValueChange.GetParameters().Length != 1)
                    {
                        Debug.LogError("Public method with name '" + contextObjectInfo.attribute.changeValueMethodName +
                        "' in '" + contextObjectInfo.component.GetType() + "' does not contain exactly [1] parameter");
                        return;
                    }
                }
                else
                {
                    Debug.LogError("Public method with name '" + contextObjectInfo.attribute.changeValueMethodName +
                        "' not found in '" + contextObjectInfo.component.GetType() + "' class");
                    return;
                }
            }

            context = Instantiate(context, contextPropertiesContainer.transform);
            context.SetInitialValues(contextObjectInfo);

            if (toggleDisableBackground) { context.GetComponent<Image>().enabled = false; toggleDisableBackground = false; }
            else toggleDisableBackground = true;

            contexts.Add(context);
        }

        void ClearContextItems()
        {
            contexts.Clear();
            foreach (Transform child in contextPropertiesContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }

        ComponentPropertyType ConvertAttributeToPropertyType(FieldInfo field)
        {
            Type fieldType = field.FieldType;
            if(PropertyContextUtils.IsNumeric(fieldType))
            {
                RangeAttribute rangeAttribute = PropertyContextUtils.FieldContainsRangeAttribute(field);
                if (rangeAttribute != null) return ComponentPropertyType.Slider;
                else return ComponentPropertyType.NumberField;
            }
            if(fieldType.IsEnum)
            {
                return ComponentPropertyType.Dropdown;
            }
            if(fieldType == typeof(bool))
            {
                return ComponentPropertyType.Toggle;
            }
            if(fieldType == typeof(Vector2Int) ||
               fieldType == typeof(Vector2))
            {
                return ComponentPropertyType.VectorField;
            }
            return ComponentPropertyType.Null;
        }
    }
}