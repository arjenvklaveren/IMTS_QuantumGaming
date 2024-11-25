using Game.Data;

namespace Game
{
    public class QuitCurrentSchemeInputDecoder : IButtonInputDecoder
    {
        public void DecodeInput(InputCode code, ButtonInputType inputType)
        {
            if (code != InputCode.Escape)
                return;

            if (inputType != ButtonInputType.Down)
                return;

            PlayerInputManager.PopInputHandler();
        }

        public void Reset() { }
        public void Destroy() { }

        private PlayerInputManager PlayerInputManager => PlayerInputManager.Instance;
    }
}
