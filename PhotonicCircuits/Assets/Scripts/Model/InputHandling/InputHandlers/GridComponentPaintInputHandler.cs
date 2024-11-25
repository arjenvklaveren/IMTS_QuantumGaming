namespace Game
{
    public class GridComponentPaintInputHandler : PlayerInputHandler
    {
        protected override IInputDecoder[] CreateInputDecoders()
        {
            return new IInputDecoder[5]
            {
                new CameraButtonMoveInputDecoder(),
                new CameraZoomInputDecoder(),
                new ComponentPaintInputDecoder(),
                new ComponentDeleteInputDecoder(),
                new QuitCurrentSchemeInputDecoder(),
            };
        }
    }
}
