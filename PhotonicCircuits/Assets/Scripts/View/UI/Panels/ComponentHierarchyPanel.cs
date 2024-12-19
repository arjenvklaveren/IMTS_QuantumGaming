using Game.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public class ComponentHierarchyPanel : Panel
    {
        [SerializeField] private GameObject componentItemContainer;
        [SerializeField] private ComponentListItem componentItemPrefab;

        [SerializeField] private GameObject componentVisualsHolder;

        private Dictionary<OpticComponent, ComponentListItem> listItems = new Dictionary<OpticComponent, ComponentListItem>();

        private void Start()
        {
            SetupListeners();
        }

        void SetupListeners()
        {
            GridController.OnGridChanged += GridController_OnGridChanged;
            GridController.OnComponentAdded += GridController_OnAddComponent;
            GridController.OnComponentRemoved += GridController_OnDeleteComponent;
            ComponentSelectionManager.Instance.OnSelectedComponent += ComponentSelectionManager_OnSelectedComponent;
        }

        void ComponentSelectionManager_OnSelectedComponent(ComponentVisuals visuals)
        {
            SyncSelectedComponentVisual();
        }

        void GridController_OnGridChanged(GridData grid)
        {
            ClearComponentItems();
            CreateComponentItems(grid);
        }

        void CreateComponentItems(GridData grid)
        {
            foreach(OpticComponent component in grid.placedComponents)
            {
                ComponentListItem listItem = Instantiate(componentItemPrefab, componentItemContainer.transform);
                listItems.Add(component, listItem);
            }
        }

        void SyncSelectedComponentVisual()
        {
            foreach(KeyValuePair<OpticComponent, ComponentListItem> item in listItems)
            {
                item.Value.SetActiveState(ComponentSelectionManager.Instance.SelectedVisuals.SourceComponent == item.Key);
            }
        }

        void GridController_OnAddComponent(OpticComponent component)
        {
            ComponentListItem item = Instantiate(componentItemPrefab, componentItemContainer.transform);
            listItems.Add(component, item);
        }

        void GridController_OnDeleteComponent(OpticComponent component)
        {
            Destroy(listItems[component].gameObject);
            listItems.Remove(component);
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
