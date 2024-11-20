using Game.Data;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game
{
    public abstract class ComponentVisuals : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public static event Action<ComponentVisuals> OnHover;

        public OpticComponent SourceComponent { get; private set; }

        private static ComponentVisuals lastHoveredVisuals;
        private static bool isHovered;

        #region Awake / Destroy
        // Visuals don't function until this is called!
        public virtual void SetSource(OpticComponent component)
        {
            SourceComponent = component;
            SetupListeners();
        }

        protected virtual void OnDestroy()
        {
            RemoveListeners();
        }

        private void SetupListeners()
        {
            PhotonVisuals.OnEnterComponent += PhotonVisuals_OnEnterComponent;
        }

        private void RemoveListeners()
        {
            PhotonVisuals.OnEnterComponent -= PhotonVisuals_OnEnterComponent;
        }
        #endregion

        #region Handle Events
        private void PhotonVisuals_OnEnterComponent(PhotonVisuals photon, OpticComponent component)
        {
            if (SourceComponent != component)
                return;

            HandlePhoton(photon);
        }
        #endregion

        protected virtual void HandlePhoton(PhotonVisuals photon) { }
        protected virtual void HandleRotationChanged(Orientation orientation) { }

        #region Hover Logic
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
        #endregion
    }
}
