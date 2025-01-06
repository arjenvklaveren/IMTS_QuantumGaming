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

        void Start()
        {
            animator = GetComponent<Animator>();
            messageText = GetComponentInChildren<TextMeshProUGUI>();
            closeButton.onClick.AddListener(() => ClosePopupPanel());
        }

        void ClosePopupPanel()
        {
            animator.Play("DefaultMessagePanel", 0, 1);
        }

        public static void DisplayMessage(string message, MessagePopupType type)
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
