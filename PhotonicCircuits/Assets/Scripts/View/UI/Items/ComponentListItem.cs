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

        Color activeStateColor;
        Color inactiveStateColor;

        bool isActive = false;

        #region Initialisation

        public void SetVisuals(string itemText, Color activeStateColor, Color inactiveStateColor, Sprite iconSprite = null, Sprite actionSprite = null)
        {
            this.itemText.text = itemText;
            this.activeStateColor = activeStateColor;
            this.inactiveStateColor = inactiveStateColor;
            if(iconSprite != null) iconImage.sprite = iconSprite;
            if(actionSprite != null) actionImage.sprite = actionSprite;
        }
        public void SetButtonActions(UnityAction mainAction, UnityAction itemAction = null, bool itemIsIdentical = false)
        {
            actionButton.onClick.AddListener(mainAction);
            if(itemAction != null) itemButton.onClick.AddListener(itemAction);
            if (itemIsIdentical) itemButton.onClick.AddListener(mainAction);
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
            if(isActive) background.color = activeStateColor;
            else background.color = inactiveStateColor;
        }
    }
}
