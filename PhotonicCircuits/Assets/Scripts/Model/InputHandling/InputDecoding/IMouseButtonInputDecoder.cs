using Game.Data;

namespace Game
{
    public interface IMouseButtonInputDecoder : IInputDecoder
    {
        public void DecodeInput(MouseInputCode code, ButtonInputType inputType);
    }
}
