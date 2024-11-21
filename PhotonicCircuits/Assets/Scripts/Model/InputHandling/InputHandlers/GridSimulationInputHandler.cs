namespace Game
{
    public class GridSimulationInputHandler : PlayerInputHandler
    {
        protected override IInputDecoder[] CreateInputDecoders()
        {
            return new IInputDecoder[2]
            {
                new CameraButtonMoveInputDecoder(),
                new CameraZoomInputDecoder(),
            };
        }
    }
}
