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

        private Dictionary<OpticComponentType, List<ComponentVisuals>> placedComponentVisuals;
        private GameObject[,] tiles;

        private void Awake()
        {
            placedComponentVisuals = new();

            SetupEventListeners();
        }

        #region Manage Event Listening
        private void SetupEventListeners()
        {
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
            AddComponent(component);
        }

        private void GridController_OnComponentRemoved(OpticComponent component)
        {
            RemoveComponent(component);
        }
        #endregion

        #region Clear Grid
        public void ClearGrid()
        {
            ClearData();
            DestroyGeneratedObjects();
        }

        private void ClearData()
        {
            placedComponentVisuals.Clear();
            tiles = null;
        }

        private void DestroyGeneratedObjects()
        {
            DestroyChildObjects(tileHolder);
            DestroyChildObjects(componentHolder);
        }
        #endregion

        #region Generate Grid
        private void GenerateGrid(GridData gridData)
        {
            currentGridSpacing = gridData.spacing;
            tiles = new GameObject[gridData.size.x, gridData.size.y];

            GenerateTiles(gridData.size);
            SetTileStates(gridData.occupiedTiles, false);

            GenerateComponents(gridData);
        }

        #region Generate Tiles
        private void GenerateTiles(Vector2Int gridSize)
        {
            for (int i = 0; i < gridSize.y; i++)
                GenerateRow(i, gridSize.x);
        }

        private void GenerateRow(int row, int size)
        {
            Transform rowHolder = new GameObject($"row_{row}").transform;
            rowHolder.SetParent(tileHolder);

            for (int i = 0; i < size; i++)
                GenerateTile(new(i, row), rowHolder);
        }

        private void GenerateTile(Vector2Int position, Transform rowHolder)
        {
            GridTile tile = Instantiate(
                tilePrefab,
                GridUtils.GridPos2WorldPos(position, currentGridSpacing),
                Quaternion.identity,
                rowHolder);

            tile.position = position;
            tile.transform.name = $"{{{position.x}, {position.y}}}";

            tiles[position.x, position.y] = tile.gameObject;
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
            ComponentVisuals prefab = GetVisualsPrefabByType(component.Type);

            ComponentVisuals visuals = Instantiate(
                prefab,
                GridUtils.GridPos2WorldPos(component.occupiedRootTile, currentGridSpacing),
                Quaternion.identity,
                componentHolder);

            visuals.SetSource(component);

            CachePlacedComponent(component, visuals);
        }

        private void CachePlacedComponent(OpticComponent source, ComponentVisuals visuals)
        {
            OpticComponentType type = source.Type;

            if (!placedComponentVisuals.ContainsKey(type))
                placedComponentVisuals.Add(type, new());

            placedComponentVisuals[type].Add(visuals);
        }
        #endregion
        #endregion

        #region Add Component
        private void AddComponent(OpticComponent component)
        {
            SetTileStates(component.occupiedTiles, false);

            GenerateComponent(component);
        }

        private void SetTileStates(HashSet<Vector2Int> tilesToSet, bool state)
        {
            foreach (Vector2Int tile in tilesToSet)
                if (IsPositionInBounds(tile))
                    tiles[tile.x, tile.y].SetActive(state);
        }
        #endregion

        #region Remove Component
        private void RemoveComponent(OpticComponent component)
        {
            if (!TryFindMatchingVisuals(component, out ComponentVisuals visuals))
                return;

            Destroy(visuals.gameObject);

            SetTileStates(component.occupiedTiles, true);
        }

        private bool TryFindMatchingVisuals(OpticComponent component, out ComponentVisuals visuals)
        {
            visuals = null;

            if (!placedComponentVisuals.ContainsKey(component.Type))
                return false;

            foreach (ComponentVisuals placedVisuals in placedComponentVisuals[component.Type])
            {
                if (placedVisuals.SourceComponent != component)
                    continue;

                visuals = placedVisuals;
                return true;
            }

            return false;
        }
        #endregion

        #region Util
        private void DestroyChildObjects(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
                Destroy(parent.GetChild(i).gameObject);
        }

        private bool IsPositionInBounds(Vector2Int position)
        {
            if (tiles == null)
                return false;

            if (tiles.Length <= position.x)
                return false;

            if (tiles.GetLength(1) <= position.y)
                return false;

            return true;
        }

        private ComponentVisuals GetVisualsPrefabByType(OpticComponentType type)
        {
            return visualsLookup.GetPrefabByComponentType(type);
        }
        #endregion
    }
}
