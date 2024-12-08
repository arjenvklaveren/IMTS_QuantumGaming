using Game.Data;
using SadUtils;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GridManager : Singleton<GridManager>
    {
        public WaitUntil WaitUntilGrid { get; private set; }

        public GridController GridController { get; private set; }

        private Stack<GridData> grids;

        protected override void Awake()
        {
            SetDefaultValues();

            SetInstance(this);
        }

        private void SetDefaultValues()
        {
            grids = new();
            GridController = new();

            WaitUntilGrid = new WaitUntil(() => grids.Count > 0);
        }

        public GridData GetActiveGrid()
        {
            return grids.Peek();
        }

        public void OpenGrid(GridData grid)
        {
            grids.Push(grid);

            GridController.SetActiveGrid(grid);
        }

        public void LoadRootGrid(GridData grid)
        {
            grids.Clear();

            OpenGrid(grid);
        }

        public void CloseActiveGrid()
        {
            if (grids.Count <= 1)
                return;

            grids.Pop();

            GridController.SetActiveGrid(grids.Peek());
        }
    }
}
