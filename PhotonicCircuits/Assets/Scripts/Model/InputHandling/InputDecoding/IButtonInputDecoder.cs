using Game.Data;

namespace Game
{
    public interface IButtonInputDecoder : IInputDecoder
    {
        public void DecodeInput(InputCode code, ButtonInputType inputType);
    }
}
