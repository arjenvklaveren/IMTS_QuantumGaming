using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    public class GridData
    {
        public event Action<OpticComponent> OnComponentAdded;
        public event Action<OpticComponent> OnComponentRemoved;

        public const int CHUNK_SIZE = 5;

        public string gridName;

        public Vector2 spacing;
        public Vector2Int size;
        public Vector2Int ChunksCount { get; private set; }

        public List<OpticComponent> placedComponents;
        public HashSet<Vector2Int> occupiedTiles;

        public bool isIntegrated;

        public Dictionary<Vector2Int, ChunkPortData> inPortsData;

        public GridData(
            string gridName,
            Vector2 spacing,
            Vector2Int size,
            bool isIntegrated = false)
        {
            this.gridName = gridName;

            this.spacing = spacing;
            this.size = size;
            CacheChunksSize();

            this.isIntegrated = isIntegrated;

            placedComponents = new();
            occupiedTiles = new();

            inPortsData = new();
        }

        private void CacheChunksSize()
        {
            Vector2 floatSize = size;

            ChunksCount = new(
                Mathf.CeilToInt(floatSize.x / CHUNK_SIZE),
                Mathf.CeilToInt(floatSize.y / CHUNK_SIZE));
        }

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
    }
}
