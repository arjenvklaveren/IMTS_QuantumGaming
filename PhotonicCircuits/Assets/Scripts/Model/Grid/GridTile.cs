using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class GridTile : MonoBehaviour, IPointerEnterHandler
    {
        public event Action<Vector2Int> OnHover;

        public Vector2Int position;

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnHover?.Invoke(position);
        }
    }
}
