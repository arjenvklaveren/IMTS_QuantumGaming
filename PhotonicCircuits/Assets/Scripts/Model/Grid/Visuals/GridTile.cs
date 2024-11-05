using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class GridTile : MonoBehaviour, IPointerEnterHandler
    {
        public static event Action<Vector2Int> OnHover;
        public static Vector2Int lastHoveredPosition;

        public Vector2Int position;

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHover?.Invoke(position);
            lastHoveredPosition = position;
        }
    }
}
