using Game.Data;
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
            ICComponentBase.OnAnyBlueprintNameChanged += ReconstructPath;
        }

        private void RemoveListeners()
        {
            GridController.OnGridChanged -= GridController_OnGridChanged;
            ICComponentBase.OnAnyBlueprintNameChanged -= ReconstructPath;
        }
        #endregion

        #region Handle Listeners
        private void GridController_OnGridChanged(GridData grid) => ReconstructPath();
        #endregion

        #region Create Path
        private void ReconstructPath()
        {
            StartCoroutine(ReconstructPathCo());
        }

        private IEnumerator ReconstructPathCo()
        {
            RemoveAllPathSteps();

            yield return null;

            List<GridData> grids = new(GridManager.Instance.GetAllGrids());
            for (int i = grids.Count - 1; i >= 0; i--)
                GeneratePathStep(grids[i], i == grids.Count - 1);

            lastPathLength = grids.Count;
            LayoutRebuilder.ForceRebuildLayoutImmediate(pathHolder);
        }
        #endregion

        #region Remove Steps
        private void RemoveAllPathSteps()
        {
            for (int i = pathHolder.childCount - 1; i >= 0; i--)
                Destroy(pathHolder.GetChild(i).gameObject);
        }
        #endregion

        #region Generate Step
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
