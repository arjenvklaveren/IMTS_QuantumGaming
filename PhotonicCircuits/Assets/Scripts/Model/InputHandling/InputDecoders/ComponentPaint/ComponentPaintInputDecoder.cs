using Game.Data;
using UnityEngine;

namespace Game
{
    public class ComponentPaintInputDecoder : IMouseButtonInputDecoder
    {
        private bool isPainting;

        #region Constructor
        public ComponentPaintInputDecoder()
        {
            SetupListeners();
        }

        private void SetupListeners()
        {
            GridTile.OnHover += GridTile_OnHover;
        }
        #endregion

        public void DecodeInput(MouseInputCode code, ButtonInputType inputType)
        {
            if (code != MouseInputCode.LeftMouseButton)
                return;

            if (inputType == ButtonInputType.Down)
                isPainting = true;
            else if (inputType == ButtonInputType.Up)
                isPainting = false;
        }

        #region Painting Event Listening
        private void GridTile_OnHover(Vector2Int position)
        {

        }
        #endregion

        public void Destroy()
        {
            GridTile.OnHover -= GridTile_OnHover;
        }
    }
}
