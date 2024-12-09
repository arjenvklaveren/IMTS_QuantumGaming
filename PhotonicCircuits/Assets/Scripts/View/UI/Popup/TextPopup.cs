using Game.Data;
using SadUtils.UI;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class TextPopup : Popup
    {
        [Header("Prefabs")]
        [SerializeField] private TMP_Text textContentPrefab;
        [SerializeField] private InputFieldHandler inputFieldContentPrefab;
        [SerializeField] private SadButton responseButtonPrefab;

        [Header("Holders")]
        [SerializeField] private TMP_Text titleLabel;
        [SerializeField] private RectTransform contentHolder;
        [SerializeField] private RectTransform buttonHolder;

        public override void Construct(PopupData data)
        {
            GenerateTitle(data);
            GenerateContents(data);
            GenerateResponseButtons(data);
            ConfigureLifeTime(data);
        }

        #region Generate Title
        private void GenerateTitle(PopupData data)
        {
            string title = data.hasTitle ? data.title : "";
            titleLabel.text = title;
        }
        #endregion

        #region Generate Content
        private void GenerateContents(PopupData data)
        {
            foreach (PopupContentData contentData in data.contents)
                GenerateContent(contentData);
        }

        private void GenerateContent(PopupContentData data)
        {
            if (!Enum.TryParse(data.Type, out PopupContentType type))
                return;

            switch (type)
            {
                case PopupContentType.Text:
                    GenerateTextContent(data as PopupTextContentData);
                    break;

                case PopupContentType.TextForm:
                    GenerateTextFormContent(data as PopupTextFormContentData);
                    break;
            }
        }

        private void GenerateTextContent(PopupTextContentData data)
        {
            TMP_Text textContent = Instantiate(textContentPrefab, contentHolder);
            textContent.text = data.text;
        }

        private void GenerateTextFormContent(PopupTextFormContentData data)
        {
            InputFieldHandler inputField = Instantiate(inputFieldContentPrefab, contentHolder);

            inputField.placeholder.text = data.placeholder;
            inputField.OnEndEdit += data.OnFormContentChanged;
        }
        #endregion

        #region Generate Response Buttons
        private void GenerateResponseButtons(PopupData data)
        {
            foreach (PopupButtonData buttonData in data.buttons)
                GenerateResponseButton(buttonData);
        }

        private void GenerateResponseButton(PopupButtonData data)
        {
            if (data.Type != "text")
                return;

            GenerateTextButton(data as PopupTextButtonData);
        }

        private void GenerateTextButton(PopupTextButtonData data)
        {
            SadButton button = Instantiate(responseButtonPrefab, buttonHolder);

            // Configure text
            TMP_Text text = button.GetComponentInChildren<TMP_Text>();
            text.text = data.text;

            // Configure onClick event
            button.OnClick += () => PopupManager.DestroyActivePopup();
            button.OnClick += data.Callback;
        }
        #endregion

        #region lifeTime
        private void ConfigureLifeTime(PopupData data)
        {
            if (!data.hasLifeTime)
                return;

            if (data.hasLifeTimeResponse)
                StartCoroutine(LifeTimeCo(data.lifeTime, data.onLifeTimeExpire));
            else
                StartCoroutine(LifeTimeCo(data.lifeTime));
        }

        private IEnumerator LifeTimeCo(float lifeTime)
        {
            yield return new WaitForSeconds(lifeTime);
            PopupManager.DestroyActivePopup();
        }

        private IEnumerator LifeTimeCo(float lifeTime, Action callback)
        {
            yield return LifeTimeCo(lifeTime);
            callback?.Invoke();
        }
        #endregion

        PopupManager PopupManager => PopupManager.Instance;
    }
}
