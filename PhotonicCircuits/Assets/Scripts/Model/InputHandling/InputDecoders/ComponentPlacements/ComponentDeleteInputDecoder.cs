using Game.Data;

namespace Game
{
    public class ComponentDeleteInputDecoder : IMouseButtonInputDecoder
    {
        private readonly GridController gridController;

        private bool isDeleting;

        #region Constructor
        public ComponentDeleteInputDecoder()
        {
            gridController = GridManager.Instance.gridController;

            SetupListeners();
        }

        private void SetupListeners()
        {
            ComponentVisuals.OnHover += ComponentVisuals_OnHover;
        }

        private void RemoveListeners()
        {
            ComponentVisuals.OnHover -= ComponentVisuals_OnHover;
        }
        #endregion

        #region Handle Input
        public void DecodeInput(MouseInputCode code, ButtonInputType inputType)
        {
            if (code != MouseInputCode.RightMouseButton)
                return;

            switch (inputType)
            {
                case ButtonInputType.Up:
                    isDeleting = false;
                    break;

                case ButtonInputType.Down:
                    isDeleting = true;
                    SendDeleteInput(ComponentVisuals.lastHoveredVisuals.sourceComponent);
                    break;
            }
        }
        #endregion

        #region Handle Events
        private void ComponentVisuals_OnHover(ComponentVisuals componentVisuals)
        {
            if (!isDeleting)
                return;

            SendDeleteInput(componentVisuals.sourceComponent);
        }
        #endregion

        private void SendDeleteInput(OpticComponent component)
        {
            gridController.TryRemoveComponent(component);
        }

        public void Destroy()
        {
            RemoveListeners();
        }
    }
}
