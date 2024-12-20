using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public class GridTile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static event Action<Vector2Int> OnHover;

        [SerializeField] private GameObject collisionObject;

        [Header("Debug")]
        [SerializeField] private bool debug;
        [SerializeField] private GameObject debugVisuals;

        [HideInInspector] public Vector2Int position;

        private static Vector2Int lastHoveredPosition;
        private static bool isHovered;

        private void Awake()
        {
            debugVisuals.SetActive(false);
        }

        #region Handle Pointer Events
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
        #endregion

        #region Get Hover Data
        public static bool TryGetHoveredPosition(out Vector2Int position)
        {
            position = lastHoveredPosition;
            return isHovered;
        }

        public static bool IsHovered => isHovered;
        #endregion

        #region Toggle Visuals

        public void ToggleState(bool state)
        {
            if (collisionObject.activeSelf == state)
                return;

            collisionObject.SetActive(state);

            if (debug)
                debugVisuals.SetActive(!state);
        }
        #endregion

        private void OnDestroy()
        {
            if (isHovered && lastHoveredPosition == position)
                isHovered = false;
        }
    }
}
