namespace Game
{
    public class GridComponentPaintInputHandler : PlayerInputHandler
    {
        protected override IInputDecoder[] CreateInputDecoders()
        {
            return new IInputDecoder[]
            {
                new CameraButtonMoveInputDecoder(),
                new CameraZoomInputDecoder(),
                new ComponentPaintInputDecoder(),
                new ComponentDeleteInputDecoder(),
            };
        }
    }
}
