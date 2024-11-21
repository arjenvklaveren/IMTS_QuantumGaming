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
            gridController = GridManager.Instance.GridController;

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
                    if (ComponentVisuals.TryGetHoveredComponent(out ComponentVisuals componentVisuals))
                        SendDeleteInput(componentVisuals);
                    break;
            }
        }
        #endregion

        #region Handle Events
        private void ComponentVisuals_OnHover(ComponentVisuals componentVisuals)
        {
            if (!isDeleting)
                return;

            SendDeleteInput(componentVisuals);
        }
        #endregion

        private void SendDeleteInput(ComponentVisuals componentVisuals)
        {
            gridController.TryRemoveComponent(componentVisuals.SourceComponent);
        }

        public void Reset()
        {
            isDeleting = false;
        }

        public void Destroy()
        {
            RemoveListeners();
        }
    }
}
