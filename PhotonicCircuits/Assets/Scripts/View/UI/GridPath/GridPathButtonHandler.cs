using Game.Data;
using SadUtils.UI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class GridPathButtonHandler : MonoBehaviour
    {
        [SerializeField] private SadButton button;
        [SerializeField] private TMP_Text label;

        private Action<string> clickCallback;
        private string gridName;

        private GridData sourceGrid;

        public void Init(GridData sourceGrid, Action<string> clickCallback, string gridName)
        {
            this.gridName = gridName;
            this.clickCallback += clickCallback;

            label.text = gridName;
            button.OnClick += HandleClick;

            this.sourceGrid = sourceGrid;
            sourceGrid.OnBlueprintNamed += Rename;
        }

        private void OnDestroy()
        {
            if (sourceGrid == null)
                return;

            sourceGrid.OnBlueprintNamed -= Rename;
        }

        private void HandleClick()
        {
            clickCallback?.Invoke(gridName);
        }

        private void Rename(string newName)
        {
            label.text = newName;

            RectTransform layoutHolder = transform.parent as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutHolder);
        }
    }
}
