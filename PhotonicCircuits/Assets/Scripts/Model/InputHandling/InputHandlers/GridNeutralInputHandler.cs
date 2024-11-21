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
            return new IInputDecoder[3]
            {
                new CameraButtonMoveInputDecoder(),
                new CameraDragInputDecoder(),
                new CameraZoomInputDecoder(),
            };
        }
        #endregion
    }
}
