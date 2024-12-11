namespace Game
{
    public class EmptyInputHandler : PlayerInputHandler
    {
        protected override IInputDecoder[] CreateInputDecoders()
        {
            return new IInputDecoder[0];
        }
    }
}
