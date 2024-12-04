using SadUtils.UI;

namespace Game.UI
{
    public class PopupTextContentData : PopupContentData
    {
        public override string Type => "text";

        public readonly string text;

        public PopupTextContentData(string text)
        {
            this.text = text;
        }
    }
}
