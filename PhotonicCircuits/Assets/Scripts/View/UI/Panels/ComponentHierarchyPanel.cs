using Game.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class ComponentHierarchyPanel : Panel
    {
        [SerializeField] private GameObject componentItemContainer;
        [SerializeField] private ComponentItem componentItemPrefab;

        private Dictionary<OpticComponent, ComponentItem> items = new Dictionary<OpticComponent, ComponentItem>();

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
                ComponentItem item = Instantiate(componentItemPrefab, componentItemContainer.transform);
                items.Add(component, item);
                item.OnCreate(component, () => OnClickItem(item));
            }
        }

        void SyncSelectedComponentVisual()
        {
            foreach(KeyValuePair<OpticComponent, ComponentItem> item in items)
            {
                if(ComponentSelectionManager.Instance.SelectedVisuals.SourceComponent == item.Key) 
                {
                    item.Value.SetBackgroundActive(true);
                } 
                else item.Value.SetBackgroundActive(false);
            }
        }

        void OnClickItem(ComponentItem item)
        {
            Debug.Log("CLICKED ITEM");
        }

        void GridController_OnAddComponent(OpticComponent component)
        {
            ComponentItem item = Instantiate(componentItemPrefab, componentItemContainer.transform);
            items.Add(component, item);
        }

        void GridController_OnDeleteComponent(OpticComponent component)
        {
            Destroy(items[component].gameObject);
            items.Remove(component);
        }

        void ClearComponentItems()
        {
            foreach(Transform child in componentItemContainer.transform)
            {
                Destroy(child.gameObject);
            }
            items.Clear();
        }
    }
}
