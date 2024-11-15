using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Game.Data;

namespace Game
{
    public class GridTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static event Action<Vector2Int> OnHover;

        public Vector2Int position;

        private static Vector2Int lastHoveredPosition;
        private static bool isHovering;

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHover?.Invoke(position);

            lastHoveredPosition = position;
            isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
        }

        public static bool TryGetHoveredPosition(out Vector2Int position)
        {
            position = lastHoveredPosition;
            return isHovering;
        }
    }
}
