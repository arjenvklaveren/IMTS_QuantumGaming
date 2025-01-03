using Game.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            GridController.OnComponentRemoved += GridController_OnDeleteComponent;
            ComponentSelectionManager.Instance.OnSelectedComponent += ComponentSelectionManager_OnSelectedComponent;
        }

        void RemoveListeners()
        {
            GridController.OnGridChanged -= GridController_OnGridChanged;
            GridController.OnComponentAdded -= GridController_OnAddComponent;
            GridController.OnComponentRemoved -= GridController_OnDeleteComponent;
            ComponentSelectionManager.Instance.OnSelectedComponent -= ComponentSelectionManager_OnSelectedComponent;
        }

        void ComponentSelectionManager_OnSelectedComponent(ComponentVisuals visuals)
        {
            SyncSelectedComponentVisual();
        }

        void GridController_OnGridChanged(GridData grid)
        {
            CreateComponentItems();
        }

        void CreateComponentItems()
        {
            ClearComponentItems();
            foreach (ComponentVisuals component in componentVisualsHolder.GetComponentsInChildren<ComponentVisuals>())
            {
                ComponentListItem listItem = Instantiate(componentItemPrefab, componentItemContainer.transform);
                ComponentPlaceDataSO placeData = placeDataLookup.GetPlaceDataByType(component.SourceComponent.Type);

                listItem.SetVisuals(placeData.title, placeData.iconSprite, rightArrowSprite);

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

        void GridController_OnDeleteComponent(OpticComponent component)
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
            foreach(Transform child in componentItemContainer.transform)
            {
                Destroy(child.gameObject);
            }
            listItems.Clear();
        }
    }
}
