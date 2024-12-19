using Game.Data;
using SadUtils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GridManager : Singleton<GridManager>
    {
        public WaitUntil WaitUntilGrid { get; private set; }

        public GridController GridController { get; private set; }

        private Stack<GridData> grids;

        private bool isSaving;

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

        public IEnumerable<GridData> GetAllGrids()
        {
            return grids;
        }

        public void OpenGrid(GridData grid)
        {
            grids.Push(grid);

            if (grid.isIntegrated)
                SaveProject(OpenCurrentGrid);

            else
                GridController.SetActiveGrid(grid);
        }

        public void LoadRootGrid(GridData grid)
        {
            grids.Clear();

            OpenGrid(grid);
        }

        public void CloseActiveGrid()
        {
            if (isSaving)
                return;

            if (grids.Count <= 1)
                return;

            CloseAndCompileActiveGrid();

            // save closed Grid
            SaveProject(OpenCurrentGrid);
        }

        public void ForceCloseActiveGrid()
        {
            CloseAndCompileActiveGrid();
        }

        private void CloseAndCompileActiveGrid()
        {
            GridData closedGrid = grids.Pop();

            ComponentPortsManager.Instance.CompileComponentPorts(closedGrid);
        }

        private void SaveProject(Action completeCallback)
        {
            isSaving = true;
            // To save integrated grid, find dirty IC component in parent grid.
            SerializationManager.Instance.SerializeProject(completeCallback);
        }

        private void OpenCurrentGrid()
        {
            GridController.SetActiveGrid(grids.Peek());

            isSaving = false;
        }
    }
}
