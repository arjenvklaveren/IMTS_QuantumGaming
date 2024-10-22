using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GridManager : MonoBehaviour
    {
        public Vector2Int lastHoveredTile;

        private Stack<GridController> grids;

        public GridController GetActiveGrid()
        {
            return grids.Peek();
        }

        public void AddGrid(GridController grid)
        {
            grids.Push(grid);
        }

        public void CloseActiveGrid()
        {
            if (grids.Count <= 1)
                return;

            grids.Pop();
        }
    }
}
