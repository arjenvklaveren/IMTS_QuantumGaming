using Game.Data;
using SadUtils;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GridManager : Singleton<GridManager>
    {
        public WaitUntil WaitUntilGrid { get; private set; }

        public GridController gridController { get; private set; }

        private Stack<GridData> grids;

        protected override void Awake()
        {
            SetDefaultValues();

            SetInstance(this);
        }

        private void SetDefaultValues()
        {
            grids = new();
            gridController = new();

            WaitUntilGrid = new WaitUntil(() => grids.Count > 0);
        }

        public GridData GetActiveGrid()
        {
            return grids.Peek();
        }

        public void OpenGrid(GridData grid)
        {
            grids.Push(grid);

            gridController.SetActiveGrid(grid);
        }

        public void CloseActiveGrid()
        {
            if (grids.Count <= 1)
                return;

            grids.Pop();

            gridController.SetActiveGrid(grids.Peek());
        }
    }
}
