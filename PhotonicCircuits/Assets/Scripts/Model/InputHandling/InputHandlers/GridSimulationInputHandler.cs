namespace Game
{
    public class GridSimulationInputHandler : PlayerInputHandler
    {
        protected override IInputDecoder[] CreateInputDecoders()
        {
            return new IInputDecoder[3]
            {
                new CameraButtonMoveInputDecoder(),
                new CameraDragInputDecoder(),
                new CameraZoomInputDecoder(),
            };
        }
    }
}
