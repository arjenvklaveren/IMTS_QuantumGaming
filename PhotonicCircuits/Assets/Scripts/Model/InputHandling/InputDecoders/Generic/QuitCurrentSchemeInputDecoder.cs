using Game.Data;

namespace Game
{
    public class QuitCurrentSchemeInputDecoder : IButtonInputDecoder
    {
        public void DecodeInput(InputCode code, ButtonInputType inputType, bool isRetroActive)
        {
            if (isRetroActive)
                return;

            if (code != InputCode.Escape)
                return;

            if (inputType != ButtonInputType.Down)
                return;

            PlayerInputManager.PopInputHandler();
        }

        public void OnDisable() { }
        public void Destroy() { }

        private PlayerInputManager PlayerInputManager => PlayerInputManager.Instance;
    }
}
