using Game.Data;
using SadUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GridPathUIHandler : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private RectTransform pathHolder;

        [Header("Prefabs")]
        [SerializeField] private GridPathButtonHandler gridPathButtonPrefab;
        [SerializeField] private RectTransform dividerPrefab;

        private int lastPathLength = 0;

        #region Awake / Destroy
        private IEnumerator Start()
        {
            yield return GridManager.WaitForInstance;
            SetupListeners();
        }

        private void OnDestroy()
        {
            if (!GridManager.HasInstance)
                return;

            RemoveListeners();
        }
        private void SetupListeners()
        {
            GridController.OnGridChanged += GridController_OnGridChanged;
        }

        private void RemoveListeners()
        {
            GridController.OnGridChanged -= GridController_OnGridChanged;
        }
        #endregion

        #region Handle Listeners
        private void GridController_OnGridChanged(GridData grid) => GeneratePath();
        #endregion

        private void GeneratePath()
        {
            IEnumerable<GridData> grids = GridManager.Instance.GetAllGrids();
            int gridCount = grids.Count();

            // Path shortened
            if (lastPathLength > gridCount)
                RemoveLastPathSteps(lastPathLength - gridCount);

            else
                GeneratePathEnd(grids, gridCount);

            lastPathLength = gridCount;
            LayoutRebuilder.ForceRebuildLayoutImmediate(pathHolder);
        }

        #region Remove Step
        private void RemoveLastPathSteps(int steps)
        {
            for (int i = 0; i < steps; i++)
                RemoveLastPathStep(i);
        }

        private void RemoveLastPathStep(int currentStep)
        {
            int startIndex = pathHolder.childCount - (2 * currentStep) - 1;

            // Remove button and divider
            for (int i = startIndex; i >= Mathf.Max(0, startIndex - 1); i--)
            {
                Destroy(pathHolder.GetChild(i).gameObject);
            }
        }
        #endregion

        #region Generate Step
        private void GeneratePathEnd(IEnumerable<GridData> grids, int gridCount)
        {
            int stepsToGenerate = gridCount - lastPathLength;
            bool isFirstStep = lastPathLength == 0;

            foreach (GridData grid in grids)
            {
                GeneratePathStep(grid, isFirstStep);

                isFirstStep = false;
                stepsToGenerate--;

                if (stepsToGenerate <= 0)
                    break;
            }
        }

        private void GeneratePathStep(GridData grid, bool isFirstStep)
        {
            if (!isFirstStep)
                GenerateSeperator();

            GeneratePathButton(grid);
        }

        private void GeneratePathButton(GridData grid)
        {
            GridPathButtonHandler buttonHandler = Instantiate(gridPathButtonPrefab, pathHolder);

            string gridName = grid.gridName;
            buttonHandler.Init(HandleButtonClick, gridName);
        }

        private void GenerateSeperator()
        {
            Instantiate(dividerPrefab, pathHolder);
        }
        #endregion

        #region Handle Button Click
        private void HandleButtonClick(string gridName)
        {
            if (GridManager.GetActiveGrid().gridName == gridName)
                return;

            StartCoroutine(HandleButtonClickCo(gridName));
        }

        private IEnumerator HandleButtonClickCo(string gridName)
        {
            while (GridManager.GetActiveGrid().gridName != gridName)
            {
                GridManager.CloseActiveGrid();
                yield return null;
            }
        }
        #endregion

        private GridManager GridManager => GridManager.Instance;
    }
}
