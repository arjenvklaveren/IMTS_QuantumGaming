using Game.Data;
using UnityEngine;

namespace Game
{
    public class ComponentPaintInputDecoder : IMouseButtonInputDecoder, IButtonInputDecoder
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
        public void DecodeInput(MouseInputCode code, ButtonInputType inputType, bool isRetroActive)
        {
            if (inputType == ButtonInputType.Hold)
                return;

            if (code == MouseInputCode.LeftMouseButton)
                HandlePaintInput(inputType);
        }

        private void HandlePaintInput(ButtonInputType inputType)
        {
            isPainting = inputType == ButtonInputType.Down;

            if (isPainting)
                if (GridTile.TryGetHoveredPosition(out Vector2Int position))
                    SendPaintInput(position);
        }

        public void DecodeInput(InputCode code, ButtonInputType inputType, bool isRetroActive)
        {
            if (inputType != ButtonInputType.Down)
                return;

            if (code == InputCode.R)
                HandleRotateInput();
        }

        private void HandleRotateInput()
        {
            ComponentPaintManager.RotatePreview();
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
