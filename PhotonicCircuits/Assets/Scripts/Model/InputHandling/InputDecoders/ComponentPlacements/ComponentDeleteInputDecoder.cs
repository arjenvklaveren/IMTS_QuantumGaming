using Game.Data;

namespace Game
{
    public class ComponentDeleteInputDecoder : IMouseButtonInputDecoder
    {
        private readonly GridController gridController;

        #region Constructor
        public ComponentDeleteInputDecoder()
        {
            gridController = GridManager.Instance.GridController;
        }
        #endregion

        #region Handle Input
        public void DecodeInput(MouseInputCode code, ButtonInputType inputType)
        {
            if (code != MouseInputCode.RightMouseButton)
                return;

            if (inputType != ButtonInputType.Down)
                return;

            if (ComponentVisuals.TryGetHoveredComponent(out ComponentVisuals componentVisuals))
                SendDeleteInput(componentVisuals);
            else
                PlayerInputManager.Instance.PopInputHandler();
        }
        #endregion

        private void SendDeleteInput(ComponentVisuals componentVisuals)
        {
            gridController.TryRemoveComponent(componentVisuals.SourceComponent);
        }

        public void OnDisable() { }
        public void Destroy() { }
    }
}
