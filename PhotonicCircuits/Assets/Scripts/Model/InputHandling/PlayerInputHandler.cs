using Game.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class PlayerInputHandler
    {
        protected IButtonInputDecoder[] buttonDecoders;
        protected IMouseButtonInputDecoder[] mouseButtonDecoders;
        protected IMousePositionDeltaInputDecoder[] mousePositionDecoders;
        protected IScrollInputDecoder[] scrollDecoders;

        #region constructor
        public PlayerInputHandler()
        {
            CacheInputDecodersByType(
                CreateInputDecoders()
            );
        }

        protected abstract IInputDecoder[] CreateInputDecoders();

        private void CacheInputDecodersByType(IInputDecoder[] decoders)
        {
            List<IButtonInputDecoder> buttonDecoderList = new();
            List<IMouseButtonInputDecoder> mouseButtonDecoderList = new();
            List<IMousePositionDeltaInputDecoder> mousePositionDecoderList = new();
            List<IScrollInputDecoder> scrollDecoderList = new();

            foreach (IInputDecoder decoder in decoders)
            {
                if (decoder is IButtonInputDecoder)
                    buttonDecoderList.Add(decoder as IButtonInputDecoder);
                if (decoder is IMouseButtonInputDecoder)
                    mouseButtonDecoderList.Add(decoder as IMouseButtonInputDecoder);
                if (decoder is IMousePositionDeltaInputDecoder)
                    mousePositionDecoderList.Add(decoder as IMousePositionDeltaInputDecoder);
                if (decoder is IScrollInputDecoder)
                    scrollDecoderList.Add(decoder as IScrollInputDecoder);
            }

            buttonDecoders = buttonDecoderList.ToArray();
            mouseButtonDecoders = mouseButtonDecoderList.ToArray();
            mousePositionDecoders = mousePositionDecoderList.ToArray();
            scrollDecoders = scrollDecoderList.ToArray();
        }
        #endregion

        #region Read Inputs
        public virtual void HandleButtonInput(InputCode code, ButtonInputType inputType)
        {
            foreach (IButtonInputDecoder decoder in buttonDecoders)
                decoder.DecodeInput(code, inputType);
        }

        public virtual void HandleMouseButtonInput(MouseInputCode code, ButtonInputType inputType)
        {
            foreach (IMouseButtonInputDecoder decoder in mouseButtonDecoders)
                decoder.DecodeInput(code, inputType);
        }

        public virtual void HandleMousePositionDelta(Vector2 mousePositionDelta)
        {
            foreach (IMousePositionDeltaInputDecoder decoder in mousePositionDecoders)
                decoder.DecodeInput(mousePositionDelta);
        }

        public virtual void HandleScrollInput(float scrollDelta)
        {
            foreach (IScrollInputDecoder decoder in scrollDecoders)
                decoder.DecodeInput(scrollDelta);
        }
        #endregion

        public virtual void OnDestroy()
        {
            // Destroy all input decoders
            foreach (IInputDecoder buttonDecoder in buttonDecoders)
                buttonDecoder.Destroy();
            foreach (IInputDecoder mouseButtonDecoder in mouseButtonDecoders)
                mouseButtonDecoder.Destroy();
            foreach (IMousePositionDeltaInputDecoder mousePositionDecoder in mousePositionDecoders)
                mousePositionDecoder.Destroy();
            foreach (IScrollInputDecoder scrollDecoder in scrollDecoders)
                scrollDecoder.Destroy();
        }
    }
}
