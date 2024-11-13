using Game.Data;
using System;
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

            foreach (Vector2Int tile in tilesToOccupy)
                if (activeGrid.occupiedTiles.Contains(tile))
                    return false;

            OpticComponent componentToAdd = placeData.CreateOpticComponent(tilesToOccupy);
            activeGrid.AddComponent(componentToAdd);

            OnComponentAdded?.Invoke(componentToAdd);
            return true;
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
    }
}
