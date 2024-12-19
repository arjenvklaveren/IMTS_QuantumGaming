using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class GridTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static event Action<Vector2Int> OnHover;

        public Vector2Int position;

        private static Vector2Int lastHoveredPosition;
        private static bool isHovered;

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHover?.Invoke(position);

            lastHoveredPosition = position;
            isHovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
        }

        public static bool TryGetHoveredPosition(out Vector2Int position)
        {
            position = lastHoveredPosition;
            return isHovered;
        }

        public static bool IsHovered => isHovered;

        private void OnDestroy()
        {
            if (isHovered && lastHoveredPosition == position)
                isHovered = false;
        }
    }
}
