using Game.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public class ComponentHierarchyPanel : Panel
    {
        [SerializeField] private GameObject componentItemContainer;
        [SerializeField] private ComponentListItem componentItemPrefab;
        [SerializeField] private Sprite rightArrowSprite;

        [SerializeField] private GameObject componentVisualsHolder;
        [SerializeField] private ComponentPlaceDataLookup placeDataLookup;

        private Dictionary<ComponentVisuals, ComponentListItem> listItems = new Dictionary<ComponentVisuals, ComponentListItem>();

        private void Start()
        {
            SetupListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        void SetupListeners()
        {
            GridController.OnGridChanged += GridController_OnGridChanged;
            GridController.OnComponentAdded += GridController_OnAddComponent;
            GridController.OnComponentRemoved += GridController_OnRemoveComponent;
            ComponentSelectionManager.Instance.OnSelectedComponent += ComponentSelectionManager_OnSelectedComponent;
            ComponentSelectionManager.Instance.OnDeselect += ComponentSelectionManager_OnDeselect;
        }

        void RemoveListeners()
        {
            GridController.OnGridChanged -= GridController_OnGridChanged;
            GridController.OnComponentAdded -= GridController_OnAddComponent;
            GridController.OnComponentRemoved -= GridController_OnRemoveComponent;
            ComponentSelectionManager.Instance.OnSelectedComponent -= ComponentSelectionManager_OnSelectedComponent;
            ComponentSelectionManager.Instance.OnDeselect -= ComponentSelectionManager_OnDeselect;
        }

        void ComponentSelectionManager_OnSelectedComponent(ComponentVisuals visuals)
        {
            SyncSelectedComponentVisual();
        }
        void ComponentSelectionManager_OnDeselect()
        {
            SyncSelectedComponentVisual();
        }

        void GridController_OnGridChanged(GridData grid)
        {
            StartCoroutine(GridChangeDelay());
        }

        IEnumerator GridChangeDelay()
        {
            yield return GridManager.Instance.WaitUntilGrid;
            CreateComponentItems();
        }

        void CreateComponentItems()
        {
            ClearComponentItems();

            foreach (ComponentVisuals component in componentVisualsHolder.GetComponentsInChildren<ComponentVisuals>())
            {
                ComponentListItem listItem = Instantiate(componentItemPrefab, componentItemContainer.transform);

                // TEST
                ComponentPlaceDataSO placeData;
                if (placeDataLookup == null)
                    listItem.SetVisuals("LOOKUP MISSING!!!", null, rightArrowSprite);
                else
                {
                    placeData = placeDataLookup.GetPlaceDataByType(component.SourceComponent.Type);
                    listItem.SetVisuals(placeData.title, placeData.iconSprite, rightArrowSprite);
                }

                UnityAction selectAction = () => { ComponentSelectionManager.Instance.SelectComponent(component); };
                listItem.SetButtonActions(selectAction, selectAction);

                listItems.Add(component, listItem);
            }

            SyncSelectedComponentVisual();
        }

        void SyncSelectedComponentVisual()
        {
            foreach (KeyValuePair<ComponentVisuals, ComponentListItem> item in listItems)
            {
                item.Value.SetActiveState(ComponentSelectionManager.Instance.SelectedVisuals == item.Key);
            }
        }

        void GridController_OnAddComponent(OpticComponent component) { CreateComponentItems(); }

        void GridController_OnRemoveComponent(OpticComponent component)
        {
            foreach (KeyValuePair<ComponentVisuals, ComponentListItem> item in listItems.ToList())
            {
                if (item.Key.SourceComponent == component)
                {
                    listItems.Remove(item.Key);
                    Destroy(item.Value.gameObject);
                }
            }
        }

        void ClearComponentItems()
        {
            for (int i = componentItemContainer.transform.childCount - 1; i >= 0; i--)
                Destroy(componentItemContainer.transform.GetChild(i).gameObject);

            listItems.Clear();
        }
    }
}
