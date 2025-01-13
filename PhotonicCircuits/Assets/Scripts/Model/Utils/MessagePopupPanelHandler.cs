using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MessagePopupPanelHandler
    {
        public static Action<string, MessagePopupType> OnDisplayMessage;

        public static void DisplayMessage(string message, MessagePopupType type)
        {
            OnDisplayMessage?.Invoke(message, type);
        }
    }
}
