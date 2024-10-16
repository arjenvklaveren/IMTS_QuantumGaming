using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GridController : MonoBehaviour
    {
        private HashSet<Vector3Int> occupiedTiles;
        private List<OpticComponent> placedComponents;

        #region Awake
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            occupiedTiles = new();
        }
        #endregion

        #region Place / Remove Components
        public bool CanPlaceComponent(OpticComponent component)
        {
            Vector3Int[] tilesToOccupy = component.GetOccupiedTiles();

            return CanPlaceComponent(tilesToOccupy);
        }

        private bool CanPlaceComponent(Vector3Int[] tilesToOccupy)
        {
            foreach (Vector3Int tile in tilesToOccupy)
                if (occupiedTiles.Contains(tile))
                    return false;

            return true;
        }

        /// <summary>
        /// Attempts to place a component on the grid. </br>
        /// This assumes that leftTopCoordinate in the component has been set.
        /// </summary>
        /// <param name="component">component to be placed</param>
        /// <returns>true if there is space. false if not.</returns>
        public bool TryPlaceComponent(OpticComponent component)
        {
            Vector3Int[] tilesToOccupy = component.GetOccupiedTiles();

            if (!CanPlaceComponent(tilesToOccupy))
                return false;

            PlaceComponent(tilesToOccupy, component);
            return true;
        }

        private void PlaceComponent(Vector3Int[] tilesToOccupy, OpticComponent component)
        {
            foreach (Vector3Int tile in tilesToOccupy)
                occupiedTiles.Add(tile);

            placedComponents.Add(component);
        }

        public void RemoveComponent(OpticComponent component)
        {
            if (!placedComponents.Contains(component))
                return;

            Vector3Int[] tilesOccupiedByComponent = component.GetOccupiedTiles();

            foreach (Vector3Int tile in tilesOccupiedByComponent)
                occupiedTiles.Remove(tile);

            placedComponents.Remove(component);
        }
        #endregion
    }
}
