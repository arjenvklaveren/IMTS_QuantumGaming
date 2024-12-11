namespace Game
{
    public class GridNeutralInputHandler : PlayerInputHandler
    {
        #region constructor
        public GridNeutralInputHandler() : base()
        {
        }

        protected override IInputDecoder[] CreateInputDecoders()
        {
            return new IInputDecoder[5]
            {
                // Camera
                new CameraButtonMoveInputDecoder(),
                new CameraDragInputDecoder(),
                new CameraZoomInputDecoder(),

                // Component Selection
                new ComponentSelectInputDecoder(),

                // Save Project
                new SaveInputDecoder(),
            };
        }
        #endregion
    }
}
