using Game.Data;

namespace Game
{
    public class SaveInputDecoder : IButtonInputDecoder
    {
        private bool ctrlModifierActive;

        #region Handle inputs
        public void DecodeInput(InputCode code, ButtonInputType inputType, bool isRetroActive)
        {
            if (code == InputCode.LeftControl)
                HandleCTRLButtonState(inputType);

            else if (code == InputCode.S)
                HandleSButtonState(inputType);
        }

        private void HandleCTRLButtonState(ButtonInputType inputType)
        {
            if (inputType == ButtonInputType.Hold)
                return;

            ctrlModifierActive = inputType == ButtonInputType.Down;
        }

        private void HandleSButtonState(ButtonInputType inputType)
        {
            if (inputType != ButtonInputType.Down)
                return;

            if (!ctrlModifierActive)
                return;

            SerializationManager.Instance.SerializeProject();
        }
        #endregion

        public void OnDisable()
        {
            ctrlModifierActive = false;
        }

        public void Destroy()
        {
        }
    }
}
