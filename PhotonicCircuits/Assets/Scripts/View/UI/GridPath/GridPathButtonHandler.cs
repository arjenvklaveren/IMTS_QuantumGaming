using SadUtils.UI;
using System;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class GridPathButtonHandler : MonoBehaviour
    {
        [SerializeField] private SadButton button;
        [SerializeField] private TMP_Text label;

        private Action<string> clickCallback;
        private string gridName;

        public void Init(Action<string> clickCallback, string gridName)
        {
            this.gridName = gridName;
            this.clickCallback += clickCallback;

            label.text = gridName;
            button.OnClick += HandleClick;
        }

        private void HandleClick()
        {
            clickCallback?.Invoke(gridName);
        }
    }
}
