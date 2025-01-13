using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CornerType = Game.WaveGuideCornerComponent.CornerType;

namespace Game
{
    public class WaveGuideCornerComponentVisuals : WaveGuideComponentVisuals
    {
        [SerializeField] private WaveGuideComponentPlaceDataSO waveguidePlaceDataFlipped;
        [SerializeField] private WaveGuideComponentPlaceDataSO waveguidePlaceDataAlt;

        [Header("Sprite references")]
        [SerializeField] SpriteRenderer cornerBase;
        [SerializeField] SpriteRenderer cornerAlt;
        [SerializeField] SpriteRenderer outlineBase;
        [SerializeField] SpriteRenderer outlineAlt;

        [Header("Component references")]
        [SerializeField] Transform pathNodeTransform;

        private WaveGuideCornerComponent sourceCorner;

        private SpriteRenderer currentCornerRenderer;
        private SpriteRenderer currentOutlineRenderer;

        private WaveGuideComponentPlaceDataSO waveguidePlaceDataCopy;

        bool hasSetVisuals;

        #region Awake / Destroy
        public override void SetSource(OpticComponent component)
        {
            base.SetSource(component);
            SetDefaultValues();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void SetDefaultValues()
        {
            waveguidePlaceDataCopy = waveguidePlaceData;
            sourceCorner = SourceComponent as WaveGuideCornerComponent;
            SetCornerVisuals(sourceCorner.cornerType);
            base.SetDefaultValues();
        }
        #endregion

        void SetCornerVisuals(CornerType cornerType)
        {
            if (hasSetVisuals) return;   
            
            bool isBaseType = (cornerType == CornerType.Default || cornerType == CornerType.Flipped);
            bool isFlippedType = cornerType == CornerType.Flipped;

            if (isBaseType) { currentCornerRenderer = cornerBase; currentOutlineRenderer = outlineBase; }
            else { currentCornerRenderer = cornerAlt; currentOutlineRenderer = outlineAlt; }

            if (isFlippedType) 
            {
                pathNodeTransform.transform.Rotate(new Vector3(0, 0, 90));
                currentCornerRenderer.flipX = isFlippedType;
                currentOutlineRenderer.flipX = isFlippedType;
            }

            ToggleTypeVisuals(isBaseType);
            SetCorrectPlaceDataRef(cornerType);
            hasSetVisuals = true;
        }

        void ToggleTypeVisuals(bool isBaseType)
        {
            cornerBase.gameObject.SetActive(isBaseType);
            outlineBase.gameObject.SetActive(isBaseType);
            cornerAlt.gameObject.SetActive(!isBaseType);
            outlineAlt.gameObject.SetActive(!isBaseType);

            ChangeOutlineSprite(currentOutlineRenderer);
        }

        void SetCorrectPlaceDataRef(CornerType cornerType)
        {
            switch (cornerType)
            {
                case CornerType.Default:
                    waveguidePlaceData = waveguidePlaceDataCopy;
                    break;
                case CornerType.Flipped:
                    waveguidePlaceData = waveguidePlaceDataFlipped;
                    break;
                case CornerType.Alternative:
                    waveguidePlaceData = waveguidePlaceDataAlt;
                    break;
            }
        }

        public override List<List<int>> NodePathsIndexesMapper()
        {
            return new List<List<int>>
            {
                new List<int> { 0, 1 }
            };
        }

        #region Handle Rotation
        protected override void HandleRotationChanged(Orientation orientation)
        {
            RotateToLookAtOrientation(visualsHolder, orientation);
        }
        #endregion 
    }
}
