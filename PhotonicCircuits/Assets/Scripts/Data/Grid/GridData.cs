using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    public class GridData
    {
        public Vector2 spacing;
        public Vector2Int size;

        public List<OpticComponent> placedComponents;
        public HashSet<Vector2Int> occupiedTiles;

        public Dictionary<Vector2Int, ChunkPortData> inPortData;

        public GridData(Vector2 spacing, Vector2Int size)
        {
            this.spacing = spacing;
            this.size = size;

            placedComponents = new();
            occupiedTiles = new();

            inPortData = new();
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
