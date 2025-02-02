using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    public class GridData
    {
        public Predicate<OpticComponent> placementCondition;

        public event Action<OpticComponent> OnComponentAdded;
        public event Action<OpticComponent> OnComponentRemoved;

        public event Action<string> OnBlueprintNamed;
        public event Action<string, string> OnBlueprintRenamed;

        public const int CHUNK_SIZE = 5;

        public string gridName;

        public Vector2 spacing;
        public Vector2Int size;
        public Vector2Int ChunksCount { get; private set; }

        public List<OpticComponent> placedComponents;
        public HashSet<Vector2Int> occupiedTiles;

        public bool isIntegrated;

        public Dictionary<Vector2Int, ChunkPortData> inPortsData;

        #region Constructors
        public GridData(
            string gridName,
            Vector2 spacing,
            Vector2Int size,
            bool isIntegrated = false)
        {
            placementCondition = CanPlaceComponent;

            this.gridName = gridName;

            this.spacing = spacing;
            this.size = size;
            CacheChunksSize();

            this.isIntegrated = isIntegrated;

            placedComponents = new();
            occupiedTiles = new();

            inPortsData = new();
        }

        // Copy constructor
        public GridData(GridData source)
        {
            placementCondition = source.placementCondition;

            gridName = source.gridName;

            spacing = source.spacing;
            size = source.size;
            CacheChunksSize();

            isIntegrated = true;

            placedComponents = new(source.placedComponents);
            occupiedTiles = new(source.occupiedTiles);

            inPortsData = new(source.inPortsData);
        }

        private void CacheChunksSize()
        {
            Vector2 floatSize = size;

            ChunksCount = new(
                Mathf.CeilToInt(floatSize.x / CHUNK_SIZE),
                Mathf.CeilToInt(floatSize.y / CHUNK_SIZE));
        }
        #endregion

        #region Default Place Condition
        private bool CanPlaceComponent(OpticComponent opticComponent) => true;
        #endregion

        #region Manage Components
        public void AddComponent(OpticComponent component)
        {
            placedComponents.Add(component);

            foreach (Vector2Int tile in component.OccupiedTiles)
                occupiedTiles.Add(tile);

            OnComponentAdded?.Invoke(component);
        }

        public void RemoveComponent(OpticComponent component)
        {
            placedComponents.Remove(component);

            foreach (Vector2Int tile in component.OccupiedTiles)
                occupiedTiles.Remove(tile);

            OnComponentRemoved?.Invoke(component);
        }
        #endregion

        #region Trigger Events
        public void TriggerNameBlueprint(string name) => OnBlueprintNamed?.Invoke(name);
        public void TriggerBlueprintRename(string oldName, string newName) => OnBlueprintRenamed?.Invoke(oldName, newName);
        #endregion
    }
}
