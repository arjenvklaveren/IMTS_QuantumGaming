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
            return new IInputDecoder[]
            {
                new CameraButtonInputDecoder(),
                new CameraDragInputDecoder(),
                new CameraScrollInputDecoder(),
            };
        }
        #endregion
    }
}
