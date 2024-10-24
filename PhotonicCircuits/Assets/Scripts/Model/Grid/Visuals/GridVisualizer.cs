using Game.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GridVisualizer : MonoBehaviour
    {
        [SerializeField] private GridTile tilePrefab;
        [SerializeField] private ComponentVisualLookupSO visualsLookup;

        [Header("Hierarchy Settings")]
        [SerializeField] private Transform tileHolder;
        [SerializeField] private Transform componentHolder;

        private Vector2 currentGridSpacing;

        private List<ComponentVisuals> placedComponentVisuals;

        #region Manage Event Listening
        private void Awake()
        {
            placedComponentVisuals = new();

            GridController.OnGridChanged += GridController_OnGridChanged;
            GridController.OnComponentAdded += GridController_OnComponentAdded;
            GridController.OnComponentRemoved += GridController_OnComponentRemoved;
        }

        private void OnDestroy()
        {
            GridController.OnGridChanged -= GridController_OnGridChanged;
            GridController.OnComponentAdded -= GridController_OnComponentAdded;
            GridController.OnComponentRemoved -= GridController_OnComponentRemoved;

        }
        #endregion

        #region Handle GridController Events
        private void GridController_OnGridChanged(GridData gridData)
        {
            ClearGrid();
            currentGridSpacing = gridData.spacing;

            GenerateGrid(gridData);
        }

        private void GridController_OnComponentAdded(OpticComponent component)
        {

        }

        private void GridController_OnComponentRemoved(OpticComponent component)
        {

        }
        #endregion

        #region Clear Grid
        public void ClearGrid()
        {
            placedComponentVisuals.Clear();

            DestroyChildren(tileHolder);
            DestroyChildren(componentHolder);
        }

        private void DestroyChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
                Destroy(parent.GetChild(i).gameObject);
        }
        #endregion

        #region Generate Grid
        public void GenerateGrid(GridData gridData)
        {
            // Generate Tiles.
            for (int row = 0; row < gridData.size.y; row++)
                GenerateRow(gridData, row);

            // Generate Components.
            GenerateComponents(gridData);
        }

        #region Generate Tiles
        private void GenerateRow(GridData gridData, int row)
        {
            Transform rowHolder = new GameObject($"row_{row}").transform;
            rowHolder.SetParent(tileHolder);

            for (int column = 0; column < gridData.size.x; column++)
                TryGenerateTile(gridData, rowHolder, new(column, row));
        }

        private void TryGenerateTile(GridData gridData, Transform holder, Vector2Int position)
        {
            if (gridData.occupiedTiles.Contains(position))
                return;

            GridTile spawnedTile = Instantiate(
                tilePrefab,
                GetTileWorldPosition(position),
                Quaternion.identity,
                holder);

            spawnedTile.position = position;
        }
        #endregion

        #region Generate Components
        private void GenerateComponents(GridData gridData)
        {
            foreach (OpticComponent component in gridData.placedComponents)
                GenerateComponent(component);
        }

        private void GenerateComponent(OpticComponent component)
        {
            ComponentVisuals visualsPrefab = visualsLookup.GetPrefabByComponentType(component.Type);

            ComponentVisuals spawnedVisuals = Instantiate(
                visualsPrefab,
                GetTileWorldPosition(component.occupiedRootTile),
                Quaternion.identity,
                componentHolder);

            spawnedVisuals.sourceComponent = component;
        }
        #endregion

        private Vector2 GetTileWorldPosition(Vector2Int gridPosition)
        {
            return gridPosition * currentGridSpacing;
        }
        #endregion
    }
}
