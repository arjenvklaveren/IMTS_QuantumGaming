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

        private void RemoveListeners()
        {
            GridTile.OnHover -= GridTile_OnHover;
        }
        #endregion

        #region Handle Input
        public void DecodeInput(MouseInputCode code, ButtonInputType inputType)
        {
            if (code != MouseInputCode.LeftMouseButton)
                return;

            switch (inputType)
            {
                case ButtonInputType.Up:
                    isPainting = false;
                    break;

                case ButtonInputType.Down:
                    isPainting = true;
                    if (GridTile.TryGetHoveredPosition(out Vector2Int position))
                        SendPaintInput(position);
                    break;
            }
        }
        #endregion

        #region Handle Events
        private void GridTile_OnHover(Vector2Int position)
        {
            if (!isPainting)
                return;

            SendPaintInput(position);
        }
        #endregion

        private void SendPaintInput(Vector2Int position)
        {
            ComponentPaintManager.PaintComponent(position);
        }

        public void OnDisable()
        {
            isPainting = false;
        }

        public void Destroy()
        {
            RemoveListeners();
            ComponentPaintManager.SelectComponent(null);
        }

        ComponentPaintManager ComponentPaintManager => ComponentPaintManager.Instance;
    }
}
