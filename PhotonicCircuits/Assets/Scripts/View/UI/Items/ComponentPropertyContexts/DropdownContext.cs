using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace Game.UI
{
    public class DropdownContext : ComponentPropertyContext
    {
        [SerializeField] Button dropdownButton;
        [SerializeField] TextMeshProUGUI selectedItemText;
        [SerializeField] GameObject dropdownContainer;
        [SerializeField] Button dropdownItemRef;

        private bool isOpen;

        private void Start()
        {
            dropdownButton.onClick.AddListener(OnClickDropdownButton);
            CloseDropDown();
        }

        protected override void OnInitialize()
        {
            CreateDropDownItems();
        }

        void CreateDropDownItems()
        {
            Type attributeEnum = contextInfo.field.FieldType;
            string[] enumNames = Enum.GetNames(attributeEnum);
            Array enumValues = Enum.GetValues(attributeEnum);

            dropdownItemRef.gameObject.SetActive(true);
            for (int i = 0; i < enumNames.Length; i++)
            {
                Button enumItem = Instantiate(dropdownItemRef, dropdownContainer.transform);
                enumItem.GetComponentInChildren<TextMeshProUGUI>().text = enumNames[i];
                var capturedEnumValue = enumValues.GetValue(i);
                enumItem.onClick.AddListener(() => OnClickDropdownItem(capturedEnumValue));
            }
            dropdownItemRef.gameObject.SetActive(false);
        }

        void DeleteDropDownItems()
        {
            foreach(Transform item in dropdownContainer.transform)
            {
                if(item.GetComponent<Button>() != dropdownItemRef) Destroy(item.gameObject);
            }
        }

        #region Click event actions
        void OnClickDropdownButton()
        {
            ToggleDropDown();
        }

        void OnClickDropdownItem(object enumValue)
        {
            ChangeEnumValue(enumValue);
            CloseDropDown();
        }
        #endregion

        void ChangeEnumValue(object enumValue)
        {
            OnEditValue(GetInvokeParams(contextInfo.onValueChange, enumValue));
        }

        #region Dropdown open/closing
        void ToggleDropDown()
        {
            isOpen = !isOpen;
            if (isOpen) OpenDropdown();
            else CloseDropDown();
        }

        void OpenDropdown()
        {
            isOpen = true;
            SetOpenState(isOpen);
        }

        void CloseDropDown()
        {
            isOpen = false;
            SetOpenState(isOpen);
        }

        void SetOpenState(bool state)
        {
            dropdownContainer.SetActive(state);
        }

        #endregion
    }
}
