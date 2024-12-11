namespace Game
{
    public class GridComponentPaintInputHandler : PlayerInputHandler
    {
        protected override IInputDecoder[] CreateInputDecoders()
        {
            return new IInputDecoder[6]
            {
                // Camera
                new CameraButtonMoveInputDecoder(),
                new CameraZoomInputDecoder(),

                // Component Painting
                new ComponentPaintInputDecoder(),
                new ComponentDeleteInputDecoder(),

                // Save Project
                new SaveInputDecoder(),

                // Exit
                new QuitCurrentSchemeInputDecoder(),
            };
        }
    }
}
