using Game.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    public class ComponentHierarchyItem : MonoBehaviour
    {
        [SerializeField] Image backgroundVisual;
        [SerializeField] Button mainButton;

        private OpticComponent component;

        public void OnCreate(OpticComponent component, UnityAction clickEvent)
        {
            this.component = component;
            mainButton.onClick.AddListener(clickEvent);
        }

        public void SetBackgroundActive(bool state)
        {
            backgroundVisual.enabled = state;
        }
    }
}
