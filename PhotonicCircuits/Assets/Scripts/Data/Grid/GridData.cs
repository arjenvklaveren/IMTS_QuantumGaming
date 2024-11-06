using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    public class GridData
    {
        public const int CHUNK_SIZE = 5;

        public Vector2 spacing;
        public Vector2Int size;
        public Vector2Int chunksSize { get; private set; }

        public List<OpticComponent> placedComponents;
        public HashSet<Vector2Int> occupiedTiles;

        public Dictionary<Vector2Int, ChunkPortData> inPortsData;

        public GridData(Vector2 spacing, Vector2Int size)
        {
            this.spacing = spacing;
            this.size = size;
            CacheChunksSize();

            placedComponents = new();
            occupiedTiles = new();

            inPortsData = new();
        }

        private void CacheChunksSize()
        {
            Vector2 floatSize = size;

            chunksSize = new(
                Mathf.CeilToInt(floatSize.x / CHUNK_SIZE),
                Mathf.CeilToInt(floatSize.y / CHUNK_SIZE));
        }

        public void AddComponent(OpticComponent component)
        {
            placedComponents.Add(component);

            foreach (Vector2Int tile in component.occupiedTiles)
                occupiedTiles.Add(tile);
        }

        public void RemoveComponent(OpticComponent component)
        {
            placedComponents.Remove(component);

            foreach (Vector2Int tile in component.occupiedTiles)
                occupiedTiles.Remove(tile);
        }
    }
}
