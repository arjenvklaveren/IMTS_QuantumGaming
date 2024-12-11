using SadUtils.UI;
using System;

namespace Game.Data
{
    public class PopupTextButtonData : PopupButtonData
    {
        public override string Type => "text";

        public override Action Callback => callback;
        private readonly Action callback;

        public readonly string text;

        public PopupTextButtonData(Action callback, string text)
        {
            this.callback = callback;
            this.text = text;
        }
    }
}
