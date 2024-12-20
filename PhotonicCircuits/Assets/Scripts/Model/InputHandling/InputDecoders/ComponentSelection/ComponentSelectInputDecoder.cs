using Game.Data;

namespace Game
{
    public class ComponentSelectInputDecoder : IMouseButtonInputDecoder
    {
        public void DecodeInput(MouseInputCode code, ButtonInputType inputType, bool isRetroActive)
        {
            if (code != MouseInputCode.LeftMouseButton)
                return;

            if (inputType != ButtonInputType.Up)
                return;

            SelectComponent();
        }

        private void SelectComponent()
        {
            if (ComponentVisuals.TryGetHoveredComponent(out ComponentVisuals componentVisuals))
                ComponentSelectionManager.SelectComponent(componentVisuals);
            else
                ComponentSelectionManager.Deselect();
        }

        public void OnDisable() { }
        public void Destroy() { }

        private ComponentSelectionManager ComponentSelectionManager => ComponentSelectionManager.Instance;

    }
}
