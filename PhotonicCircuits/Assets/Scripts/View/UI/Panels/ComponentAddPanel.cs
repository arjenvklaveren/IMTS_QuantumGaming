using Codice.CM.SEIDInfo;
using Game.Data;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField] ComponentListItem listItemPrefab;

        [Header("Visuals references")]
        [SerializeField] Sprite addCrossSprite;
        [SerializeField] Sprite blueprintIconSprite;

        private bool isOpen;
        private bool isComponentList = true;

        Dictionary<ComponentListItem, ComponentPlaceDataSO> currentPlaceDataList = new Dictionary<ComponentListItem, ComponentPlaceDataSO>();

        #region Initialisation
        private void Start()
        {
            AddListeners();
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
            ComponentPaintManager.Instance.OnPlaceDataChanged += ComponentPaintManager_OnPlaceDataChanged;
            GridController.OnGridChanged += GridController_OnGridChanged;
        }

        void RemoveListeners()
        {
            toggleButtonMain.onClick.RemoveListener(TogglePanel);
            toggleButtonBox.onClick.RemoveListener(TogglePanel);
            selectComponentListButton.onClick.RemoveListener(ToggleListType);
            selectBlueprintListButton.onClick.RemoveListener(ToggleListType);
            ComponentPaintManager.Instance.OnPlaceDataChanged -= ComponentPaintManager_OnPlaceDataChanged;
            GridController.OnGridChanged -= GridController_OnGridChanged;
        }
        #endregion

        void TogglePanel()
        {
            if (AnimatorIsPlaying()) return;
            isOpen = !isOpen;
            if (isOpen) animator.Play("OpenAddComponentBox");
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

        void GridController_OnGridChanged(GridData data) 
        {
            if (isComponentList)
            {
                DestroyCurrentListItems();
                GenerateComponentList();
            }
        }

        void GenerateComponentList()
        {
            bool currentGridIsIntegrated = GridManager.Instance.GetActiveGrid().isIntegrated;
            foreach (ComponentPlaceDataSO placeData in allComponentPlaceData)
            {
                bool isFreeSpace = placeData.restrictionType == PlaceRestrictionType.FreeSpaceOnly || placeData.restrictionType == PlaceRestrictionType.Both;

                if (!currentGridIsIntegrated != isFreeSpace) continue;

                ComponentListItem listItem = Instantiate(listItemPrefab, listItemHolder.transform);
                UnityAction mainAction = () => ComponentPaintManager.Instance.SelectComponent(placeData);
                listItem.SetButtonActions(mainAction, mainAction);
                listItem.SetVisuals(placeData.title, placeData.iconSprite, addCrossSprite);
                currentPlaceDataList.Add(listItem, placeData);
            }
        }

        void GenerateBlueprintList()
        {
            foreach (string blueprintName in ICBlueprintManager.Instance.GetAllBlueprintNames())
            {
                ComponentListItem listItem = Instantiate(listItemPrefab, listItemHolder.transform);
                UnityAction mainAction = () => ComponentPaintManager.Instance.SelectBlueprint(blueprintName);
                listItem.SetButtonActions(mainAction, mainAction);
                listItem.SetVisuals(blueprintName, blueprintIconSprite, addCrossSprite);
                currentPlaceDataList.Add(listItem, null);
            }
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

        void ComponentPaintManager_OnPlaceDataChanged(ComponentPlaceDataSO placeData) => VisualiseSelectedItem(placeData);

        void VisualiseSelectedItem(ComponentPlaceDataSO placeData)
        {
            foreach(KeyValuePair<ComponentListItem, ComponentPlaceDataSO> data in currentPlaceDataList)
            {
                if (placeData == null) { data.Key.SetActiveState(false); continue; }
                if(isComponentList) data.Key.SetActiveState(placeData == data.Value);
                else data.Key.SetActiveState(placeData.title == data.Key.GetItemName());
            }
        }

        void SetListImageAlpha(Image image, float alpha)
        {
            Color color = image.color;
            image.color = new Color(color.r, color.g, color.b, alpha);
        }

        void DestroyCurrentListItems()
        {
            foreach (Transform child in listItemHolder.transform) Destroy(child.gameObject);
            currentPlaceDataList.Clear();
        }

        bool AnimatorIsPlaying()
        {
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
            return (currentState.normalizedTime < 1.0f && currentState.length > 0);
        }
    }
}
