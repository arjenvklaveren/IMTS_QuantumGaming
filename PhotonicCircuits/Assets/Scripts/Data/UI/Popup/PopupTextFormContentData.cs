using SadUtils.UI;
using System;

namespace Game.Data
{
    public class PopupTextFormContentData : PopupContentData
    {
        public override string Type => PopupContentType.TextForm.ToString();

        public readonly Action<string> OnFormContentChanged;

        public readonly string placeholder;

        public PopupTextFormContentData(
            Action<string> OnFormContentChanged,
            string preview)
        {
            this.OnFormContentChanged = OnFormContentChanged;
            this.placeholder = preview;
        }
    }
}
