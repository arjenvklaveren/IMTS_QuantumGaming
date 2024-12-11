using SadUtils.UI;

namespace Game.Data
{
    public class PopupTextContentData : PopupContentData
    {
        public override string Type => PopupContentType.Text.ToString();

        public readonly string text;

        public PopupTextContentData(string text)
        {
            this.text = text;
        }
    }
}
