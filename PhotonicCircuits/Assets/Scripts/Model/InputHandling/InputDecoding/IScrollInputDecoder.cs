namespace Game
{
    public interface IScrollInputDecoder : IInputDecoder
    {
        public void DecodeInput(float scrollDelta);
    }
}
