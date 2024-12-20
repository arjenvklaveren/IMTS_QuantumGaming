using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    public class ComponentListItem : MonoBehaviour
    {
        [Header("Visuals references")]
        [SerializeField] Image iconImage;
        [SerializeField] Image actionImage;
        [SerializeField] TextMeshProUGUI itemText;
        [SerializeField] Image background;

        [Header("Button references")]
        [SerializeField] Button itemButton;
        [SerializeField] Button actionButton;

        bool isActive = false;

        #region Initialisation

        public void SetVisuals(string itemText, Sprite iconSprite = null, Sprite actionSprite = null)
        {
            this.itemText.text = itemText;
            if(iconSprite != null) iconImage.sprite = iconSprite;
            if(actionSprite != null) actionImage.sprite = actionSprite;
        }
        public void SetButtonActions(UnityAction mainAction, UnityAction itemAction = null)
        {
            actionButton.onClick.AddListener(mainAction);
            if(itemAction != null) itemButton.onClick.AddListener(itemAction);
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        void RemoveListeners()
        {
            actionButton.onClick.RemoveAllListeners();
            itemButton.onClick.RemoveAllListeners();
        }
        #endregion


        public void SetActiveState(bool state)
        {
            isActive = state;
            background.enabled = state;
        }
        public bool GetActiveState() { return isActive; }
    }
}
