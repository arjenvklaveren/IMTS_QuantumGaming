using Game.Data;
using SadUtils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class GridManager : Singleton<GridManager>
    {
        public event Action<string> OnProjectRenamed;

        public WaitUntil WaitUntilGrid { get; private set; }

        public GridController GridController { get; private set; }

        private Stack<GridData> grids;

        private bool isSaving;

        #region Awake
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
        #endregion

        #region Get Grid Data
        public GridData GetActiveGrid()
        {
            return grids.Peek();
        }

        public IEnumerable<GridData> GetAllGrids()
        {
            return grids;
        }
        #endregion

        #region Open Grid Data
        public void OpenProject(GridData rootGrid)
        {
            SaveProject(() => OpenProjectAfterSave(rootGrid));
        }

        public void OpenGrid(GridData grid)
        {
            if (grid.isIntegrated)
                SaveProject(() => OpenGridAfterSave(grid));

            else
            {
                grids.Push(grid);
                GridController.SetActiveGrid(grid);
            }
        }

        public void ForceOpenGrid(GridData grid)
        {
            grids.Push(grid);
            GridController.SetActiveGrid(grid);
        }
        #endregion

        #region Close Grid
        public void CloseActiveGrid()
        {
            if (isSaving)
                return;

            if (grids.Count <= 1)
                return;

            // save closed Grid
            SaveProject(CloseGridAfterSave);
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
        #endregion

        #region Handle Project Save
        private void SaveProject(Action completeCallback)
        {
            if (isSaving)
                return;

            isSaving = true;
            // To save integrated grid, find dirty IC component in parent grid.
            SerializationManager.Instance.SerializeProject(completeCallback);
        }

        private void OpenGridAfterSave(GridData grid)
        {
            //Check if grid host to open still exists
            if (CanOpenGrid(grid))
                grids.Push(grid);

            OpenCurrentGrid();
        }

        private bool CanOpenGrid(GridData grid)
        {
            GridData hostGrid = grids.Peek();

            foreach (OpticComponent component in hostGrid.placedComponents)
            {
                if (!IsICBlueprintComponent(component.Type))
                    continue;

                ICComponentBase icComponent = component as ICComponentBase;

                if (ReferenceEquals(icComponent.InternalGrid, grid))
                    return true;
            }

            return false;
        }

        private bool IsICBlueprintComponent(OpticComponentType type)
        {
            int typeId = (int)type;
            return typeId >= 100 && typeId < 200;
        }

        private void CloseGridAfterSave()
        {
            CloseAndCompileActiveGrid();

            OpenCurrentGrid();
        }

        private void OpenProjectAfterSave(GridData rootGrid)
        {
            grids.Clear();

            grids.Push(rootGrid);

            OpenCurrentGrid();
        }

        private void OpenCurrentGrid()
        {
            GridController.SetActiveGrid(grids.Peek());

            isSaving = false;
        }
        #endregion

        #region Trigger Events
        public void TriggerProjectRenamed(string newName) => OnProjectRenamed?.Invoke(newName);
        #endregion
    }
}
