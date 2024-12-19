using Game.Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ComponentAddPanel : Panel
    {
        [Header("Toggling references")]
        [SerializeField] Button toggleButtonMain;
        [SerializeField] Button toggleButtonBox;
        [SerializeField] Animator animator;

        [Header("Main references")]
        [SerializeField] Button selectComponentListButton;
        [SerializeField] Button selectBlueprintListButton;
        [SerializeField] GameObject listItemHolder;

        [Header("External references")]
        [SerializeField] List<ComponentPlaceDataSO> allComponentPlaceData = new List<ComponentPlaceDataSO>();
        [SerializeField] GameObject listItemPrefab;

        private bool isOpen;
        private bool isComponentList = true;

        ComponentPlaceDataSO currentPlace;

        #region Initialisation
        private void Start()
        {
            AddListeners();
            GenerateComponentList();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        void AddListeners()
        {
            toggleButtonMain.onClick.AddListener(TogglePanel);
            toggleButtonBox.onClick.AddListener(TogglePanel);
            selectComponentListButton.onClick.AddListener(ToggleListType);
            selectBlueprintListButton.onClick.AddListener(ToggleListType);
        }

        void RemoveListeners()
        {
            toggleButtonMain.onClick.RemoveListener(TogglePanel);
            toggleButtonBox.onClick.RemoveListener(TogglePanel);
            selectComponentListButton.onClick.RemoveListener(ToggleListType);
            selectBlueprintListButton.onClick.RemoveListener(ToggleListType);
        }
        #endregion

        void TogglePanel()
        {
            if (AnimatorIsPlaying()) return;
            isOpen = !isOpen;
            if(isOpen) animator.Play("OpenAddComponentBox");
            else animator.Play("CloseAddComponentBox");
        }

        void ToggleListType()
        {
            isComponentList = !isComponentList;
            DestroyCurrentListItems();
            if (isComponentList) GenerateComponentList();
            else GenerateBlueprintList();
            VisualiseSelectedList();
        }

        void GenerateComponentList()
        {
            foreach(ComponentPlaceDataSO placeData in allComponentPlaceData)
            {
                GameObject listItem = Instantiate(listItemPrefab, listItemHolder.transform);
                Button itemButton = listItem.GetComponentInChildren<Button>();
                TextMeshProUGUI itemText = listItem.GetComponentInChildren<TextMeshProUGUI>();
                itemButton.onClick.AddListener(() => ComponentPaintManager.Instance.SelectComponent(placeData));
                itemText.text = placeData.title;
            }
        }

        void GenerateBlueprintList()
        {

        }

        void VisualiseSelectedList()
        {
            Image componentListImage = selectComponentListButton.transform.GetComponent<Image>();
            Image blueprintListImage = selectBlueprintListButton.transform.GetComponent<Image>();

            float activeAlpha = 1f;
            float inactiveAlpha = 0.25f;

            SetListImageAlpha(componentListImage, isComponentList ? activeAlpha : inactiveAlpha);
            SetListImageAlpha(blueprintListImage, isComponentList ? inactiveAlpha : activeAlpha);
        }

        void SetListImageAlpha(Image image, float alpha)
        {
            Color color = image.color;
            image.color = new Color(color.r, color.g, color.b, alpha);
        }

        void DestroyCurrentListItems()
        {
            foreach(Transform child in listItemHolder.transform) Destroy(child.gameObject);
        }

        bool AnimatorIsPlaying()
        {
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
            return (currentState.normalizedTime < 1.0f && currentState.length > 0);
        }
    }
}