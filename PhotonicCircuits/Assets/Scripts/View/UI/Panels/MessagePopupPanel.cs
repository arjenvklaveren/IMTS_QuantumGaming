using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class MessagePopupPanel : Panel
    {
        [SerializeField] Button closeButton;
        static Animator animator;
        static TextMeshProUGUI messageText;

        #region Initialisation
        void Start()
        {
            animator = GetComponent<Animator>();
            messageText = GetComponentInChildren<TextMeshProUGUI>();
            SetupListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();   
        }

        void SetupListeners()
        {
            closeButton.onClick.AddListener(() => ClosePopupPanel());
            MessagePopupPanelHandler.OnDisplayMessage += DisplayMessage;
        }

        void RemoveListeners()
        {
            closeButton.onClick.RemoveListener(() => ClosePopupPanel());
            MessagePopupPanelHandler.OnDisplayMessage -= DisplayMessage;
        }
        #endregion

        void ClosePopupPanel()
        {
            animator.Play("DefaultMessagePanel", 0, 1);
        }

        public void DisplayMessage(string message, MessagePopupType type)
        {
            animator.Play("DefaultMessagePanel", 0, 1);
            animator.Update(0);
            string startText = type.ToString();
            messageText.text = GetStartTextColor(type) + startText + ": <color=white>" + message; 
            animator.Play("SlideMessagePanel");
        }

        public static string GetStartTextColor(MessagePopupType type)
        {
            return type switch
            {
                MessagePopupType.Error => "<color=red>",
                MessagePopupType.Info => "<color=yellow>",
                MessagePopupType.Success => "<color=green>",
                MessagePopupType.DebugError => "<color=purple>",
                _ => "<color=white>"
            };
        }
    }
}
