using Game.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GridController
    {
        public static event Action<GridData> OnGridChanged;
        public static event Action<OpticComponent> OnComponentAdded;
        public static event Action<OpticComponent> OnComponentRemoved;

        private GridData activeGrid;

        public void SetActiveGrid(GridData grid)
        {
            activeGrid = grid;

            OnGridChanged?.Invoke(grid);
        }

        #region Add Component
        public bool TryAddComponent(ComponentPlaceDataSO placeData, Vector2Int position)
        {
            Vector2Int[] tilesToOccupy = placeData.GetTilesToOccupy(position);

            if (!AreTilesInBounds(tilesToOccupy))
                return false;

            if (!AreTilesEmpty(tilesToOccupy))
                return false;

            OpticComponent componentToAdd = placeData.CreateOpticComponent(activeGrid, tilesToOccupy);
            AddComponent(componentToAdd);

            return true;
        }

        private bool AreTilesInBounds(IEnumerable<Vector2Int> tilesToOccupy)
        {
            foreach (Vector2Int tile in tilesToOccupy)
                if (GridUtils.IsPosOutOfBounds(tile, activeGrid))
                    return false;

            return true;
        }

        private bool AreTilesEmpty(IEnumerable<Vector2Int> tilesToOccupy)
        {
            foreach (Vector2Int tile in tilesToOccupy)
                if (activeGrid.occupiedTiles.Contains(tile))
                    return false;

            return true;
        }

        private void AddComponent(OpticComponent component)
        {
            activeGrid.AddComponent(component);
            OnComponentAdded?.Invoke(component);
        }
        #endregion

        #region Remove Component
        public bool TryRemoveComponent(OpticComponent component)
        {
            if (!activeGrid.placedComponents.Contains(component))
                return false;

            activeGrid.RemoveComponent(component);
            component.Destroy();

            OnComponentRemoved?.Invoke(component);
            return true;
        }
        #endregion

        #region Rotate Component
        public bool TryRotateComponentClockwise(OpticComponent component, int increments = 1)
        {
            if (!activeGrid.placedComponents.Contains(component))
                return false;

            // Remove Component
            activeGrid.RemoveComponent(component);
            OnComponentRemoved?.Invoke(component);

            // Rotate Component
            component.RotateClockwise(increments);

            // Make sure Component still fits on grid
            while (!AreTilesEmpty(component.OccupiedTiles))
                component.RotateClockwise();

            // Add Component
            AddComponent(component);

            return true;
        }
        #endregion
    }
}
