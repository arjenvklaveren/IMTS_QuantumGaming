using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class ComponentVisuals : MonoBehaviour, IPointerEnterHandler
    {
        public static event Action<ComponentVisuals> OnHover;
        public static ComponentVisuals lastHoveredVisuals;

        public OpticComponent sourceComponent;

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHover?.Invoke(this);
            lastHoveredVisuals = this;
        }
    }
}
