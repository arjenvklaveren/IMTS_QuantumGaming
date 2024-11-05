using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class ComponentVisuals : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static event Action<ComponentVisuals> OnHover;

        public OpticComponent sourceComponent;

        private static ComponentVisuals lastHoveredVisuals;
        private static bool isHovered;

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHover?.Invoke(this);

            lastHoveredVisuals = this;
            isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
        }

        public static bool TryGetHoveredComponent(out ComponentVisuals componentVisuals)
        {
            componentVisuals = lastHoveredVisuals;
            return isHovered;
        }
    }
}
