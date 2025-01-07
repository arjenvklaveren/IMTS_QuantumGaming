using Game.Data;
using SadUtils;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class PaintPreviewHandler : MonoBehaviour
    {
        [Header("Preview Settings")]
        [SerializeField] private Color previewColor;
        [SerializeField] private float tileOffsetSize;

        [Header("Refs")]
        [SerializeField] private RectTransform previewHolder;
        [SerializeField] private RectTransform imageHolder;
        [SerializeField] private Image previewImage;

        private Vector2 defaultImageSizeDelta;

        private Orientation currentDefaultOrientation;

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

            defaultImageSizeDelta = imageHolder.sizeDelta;
            previewImage.color = previewColor;
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void SetupListeners()
        {
            ComponentPaintManager.OnPlaceDataChanged += ComponentPaintManager_OnPlaceDataChanged;
            ComponentPaintManager.OnOrientationOffsetChanged += ComponentPaintManager_OnOrientationOffsetChanged;
        }

        private void RemoveListeners()
        {
            ComponentPaintManager.OnPlaceDataChanged -= ComponentPaintManager_OnPlaceDataChanged;
            ComponentPaintManager.OnOrientationOffsetChanged -= ComponentPaintManager_OnOrientationOffsetChanged;
        }
        #endregion

        #region Handle Events
        private void ComponentPaintManager_OnPlaceDataChanged(ComponentPlaceDataSO placeData) => HandlePlaceDataPreview(placeData);
        private void ComponentPaintManager_OnOrientationOffsetChanged(Orientation orientation) => HandleOrientationChanged(orientation);

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
            currentDefaultOrientation = placeData.defaultOrientation;

            ConfigureImageSprite(placeData.previewSprite);
            imageHolder.sizeDelta = defaultImageSizeDelta * placeData.previewScale;

            previewImage.rectTransform.anchoredPosition = (Vector2)placeData.previewTileOffset * tileOffsetSize;

            if (!previewHolder.gameObject.activeSelf)
            {
                previewHolder.gameObject.SetActive(true);
                enabled = true;
            }
        }

        private void ConfigureImageSprite(Sprite previewSprite)
        {
            previewImage.sprite = previewSprite;

            previewImage.type = previewImage.hasBorder ? Image.Type.Sliced : Image.Type.Simple;
        }

        private void HandleOrientationChanged(Orientation offsetOrientation)
        {
            int offsetIncrements = (int)offsetOrientation;
            Orientation placeOrientation = currentDefaultOrientation.RotateClockwise(offsetIncrements);

            Vector3 targetLookAt = (Vector3)placeOrientation.ToVector2();
            imageHolder.rotation = LookAt2D.GetLookAtRotation(Vector3.zero, targetLookAt);
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
