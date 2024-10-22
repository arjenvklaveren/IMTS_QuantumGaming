using Game.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GridController
    {
        private readonly HashSet<Vector2Int> occupiedTiles;
        private readonly List<OpticComponent> placedComponents;

        private readonly GridGenerationData gridGenerationData;

        #region Contructor
        public GridController(GridGenerationData gridGenerationData)
        {
            this.gridGenerationData = gridGenerationData;

            occupiedTiles = new();
            placedComponents = new();
        }
        #endregion

        #region Place Components
        public bool CanPlaceComponent(OpticComponentPreview preview, Vector2Int position)
        {
            return CanPlaceComponent(preview.GetTilesToOccupy(position));
        }

        private bool CanPlaceComponent(Vector2Int[] tilesToOccupy)
        {
            foreach (Vector2Int tile in tilesToOccupy)
                if (occupiedTiles.Contains(tile))
                    return false;

            return true;
        }

        public bool TryPlaceComponent(OpticComponentPreview preview, Vector2Int position)
        {
            Vector2Int[] tilesToOccupy = preview.GetTilesToOccupy(position);

            if (!CanPlaceComponent(tilesToOccupy))
                return false;

            PlaceComponent(preview, tilesToOccupy);
            return true;
        }

        private void PlaceComponent(OpticComponentPreview preview, Vector2Int[] tilesToOccupy)
        {
            foreach (Vector2Int tile in tilesToOccupy)
                occupiedTiles.Add(tile);

            OpticComponent spawnedComponent = SpawnComponentVisuals(preview.componentPrefab, tilesToOccupy[0]);

            spawnedComponent.occupiedTiles = new(tilesToOccupy);

            placedComponents.Add(spawnedComponent);
        }

        private OpticComponent SpawnComponentVisuals(OpticComponent componentPrefab, Vector2Int position)
        {
            Vector2 spawnPosition = position * gridGenerationData.spacing;
            return Object.Instantiate(componentPrefab, spawnPosition, Quaternion.identity);
        }
        #endregion

        #region Remove Components
        public void RemoveComponent(OpticComponent component)
        {
            if (!placedComponents.Contains(component))
                return;

            placedComponents.Remove(component);

            foreach (Vector2Int tile in component.occupiedTiles)
                occupiedTiles.Remove(tile);

            Object.Destroy(component.gameObject);
        }

        public void RemoveComponent(Vector2Int position)
        {
            if (!GetComponentByPosition(position, out OpticComponent component))
                return;

            RemoveComponent(component);
        }
        #endregion

        #region Util
        public bool GetComponentByPosition(Vector2Int position, out OpticComponent component)
        {
            component = null;

            if (!occupiedTiles.Contains(position))
                return false;

            foreach (OpticComponent placedComponent in placedComponents)
            {
                if (placedComponent.occupiedTiles.Contains(position))
                {
                    component = placedComponent;
                    break;
                }
            }

            return true;
        }
        #endregion
    }
}
