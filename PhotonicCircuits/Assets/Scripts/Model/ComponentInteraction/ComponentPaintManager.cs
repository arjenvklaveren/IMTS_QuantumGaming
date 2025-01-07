using Game.Data;
using SadUtils;
using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class ComponentPaintManager : Singleton<ComponentPaintManager>
    {
        public event Action<ComponentPlaceDataSO> OnPlaceDataChanged;
        public event Action<Orientation> OnOrientationOffsetChanged;

        [Header("Blueprint Settings")]
        public UnityDictionary<OpticComponentType, ComponentPlaceDataSO> placeDatas;

        private ComponentPlaceDataSO selectedComponent;
        private Orientation orientationOffset;

        private GridController gridController;

        private bool canRotateComponent;

        protected override void Awake()
        {
            SetInstance(this);
        }

        private IEnumerator Start()
        {
            yield return GridManager.WaitForInstance;
            gridController = GridManager.Instance.GridController;
        }

        #region Rotate Preview
        public void RotatePreview()
        {
            if (!canRotateComponent)
                return;

            orientationOffset = orientationOffset.RotateClockwise();

            OnOrientationOffsetChanged?.Invoke(orientationOffset);
        }
        #endregion

        #region Paint Component
        public void PaintComponent(Vector2Int position)
        {
            if (gridController.TryAddComponent(selectedComponent, position, orientationOffset))
                return;

            // Show error
            Debug.Log($"can't paint at {position}");
        }
        #endregion

        #region Select Place Data
        public void SelectComponent(OpticComponentType type)
        {
            if (!placeDatas.ContainsKey(type))
                return;

            SelectComponent(placeDatas[type]);
        }

        public void SelectBlueprint(string blueprintName)
        {
            if (!ICBlueprintManager.Instance.TryGetBlueprintData(blueprintName, out ICBlueprintData blueprintData))
                return;

            if (!placeDatas.ContainsKey(blueprintData.type))
                return;

            ICBlueprintPlaceDataSO blueprintPlaceData = ScriptableObject.CreateInstance<ICBlueprintPlaceDataSO>();

            ComponentPlaceDataSO templatePlaceData = placeDatas[blueprintData.type];

            SetBlueprintPlaceDataDefaultValues(
                blueprintPlaceData,
                templatePlaceData,
                blueprintName);

            blueprintPlaceData.SetBlueprintReference(templatePlaceData, blueprintData);

            SelectComponent(blueprintPlaceData);
        }

        private void SetBlueprintPlaceDataDefaultValues(
            ICBlueprintPlaceDataSO blueprintPlaceData,
            ComponentPlaceDataSO templatePlaceData,
            string blueprintName)
        {
            blueprintPlaceData.title = blueprintName;

            blueprintPlaceData.previewScale = templatePlaceData.previewScale;
            blueprintPlaceData.previewSprite = templatePlaceData.previewSprite;
            blueprintPlaceData.previewTileOffset = templatePlaceData.previewTileOffset;

            blueprintPlaceData.defaultOrientation = templatePlaceData.defaultOrientation;
        }

        public void SelectComponent(ComponentPlaceDataSO placeData)
        {
            // Set paint control scheme.
            TrySetPaintMode();

            if (placeData != null)
                canRotateComponent = placeData.canRotate;

            selectedComponent = placeData;
            OnPlaceDataChanged?.Invoke(placeData);
            OnOrientationOffsetChanged?.Invoke(orientationOffset);
        }

        private void TrySetPaintMode()
        {
            if (selectedComponent == null)
                PlayerInputManager.AddInputHandler(new GridComponentPaintInputHandler());
        }
        #endregion

        private PlayerInputManager PlayerInputManager => PlayerInputManager.Instance;
    }
}
