using Game.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PaintPreviewHandler : MonoBehaviour
    {
        [Header("Preview Settings")]
        [SerializeField] private Color previewColor;

        [Header("Refs")]
        [SerializeField] private RectTransform previewHolder;
        [SerializeField] private Image previewImage;

        private Vector2 defaultImageSizeDelta;

        #region Start / Destroy
        private void Start()
        {
            SetDefaultValues();
            SetupListeners();
        }

        private void SetDefaultValues()
        {
            enabled = false;
            previewHolder.gameObject.SetActive(false);

            defaultImageSizeDelta = previewImage.rectTransform.sizeDelta;
            previewImage.color = previewColor;
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void SetupListeners()
        {
            ComponentPaintManager.onPlaceDataChanged += ComponentPaintManager_onPlaceDataChanged;
        }

        private void RemoveListeners()
        {
            ComponentPaintManager.onPlaceDataChanged -= ComponentPaintManager_onPlaceDataChanged;
        }
        #endregion

        #region Handle Events
        private void ComponentPaintManager_onPlaceDataChanged(ComponentPlaceDataSO placeData) => HandlePlaceDataPreview(placeData);

        private void HandlePlaceDataPreview(ComponentPlaceDataSO placeData)
        {
            if (placeData == null)
                RemovePreview();
            else
                UpdatePreview(placeData);
        }

        private void RemovePreview()
        {
            previewHolder.gameObject.SetActive(false);
            enabled = false;
        }

        private void UpdatePreview(ComponentPlaceDataSO placeData)
        {
            previewImage.sprite = placeData.previewSprite;
            previewImage.rectTransform.sizeDelta = defaultImageSizeDelta * placeData.previewScale;

            if (!previewHolder.gameObject.activeSelf)
            {
                previewHolder.gameObject.SetActive(true);
                enabled = true;
            }
        }
        #endregion

        #region Update Loop
        private void Update()
        {
            MovePreview();
        }

        private void MovePreview()
        {
            previewHolder.position = Input.mousePosition;
        }
        #endregion

        private ComponentPaintManager ComponentPaintManager => ComponentPaintManager.Instance;
    }
}
