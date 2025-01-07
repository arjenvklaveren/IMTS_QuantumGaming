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
        [SerializeField] SpriteRenderer cornerBase;
        [SerializeField] SpriteRenderer cornerAlt;
        [SerializeField] SpriteRenderer outlineBase;
        [SerializeField] SpriteRenderer outlineAlt;

        private WaveGuideCornerComponent sourceCorner;

        private SpriteRenderer currentCornerRenderer;
        private SpriteRenderer currentOutlineRenderer;

        #region Awake / Destroy
        public override void SetSource(OpticComponent component)
        {
            base.SetSource(component);
            SetDefaultValues();
            SetupListeners();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveListeners();
        }

        protected override void SetDefaultValues()
        {
            base.SetDefaultValues();
            sourceCorner = SourceComponent as WaveGuideCornerComponent;
        }

        private void SetupListeners()
        {
            sourceCorner.OnChangeCornerType += Source_OnChangeCornerType;
        }

        private void RemoveListeners()
        {
            sourceCorner.OnChangeCornerType -= Source_OnChangeCornerType;
        }
        #endregion

        private void Source_OnChangeCornerType(CornerType cornerType)
        {
            SetCornerVisuals(cornerType);
        }

        void SetCornerVisuals(CornerType cornerType)
        {
            bool isBaseType = (cornerType == CornerType.Default || cornerType == CornerType.DefaultFlipped);
            bool isFlippedType = (cornerType == CornerType.DefaultFlipped);
            if (isBaseType) { currentCornerRenderer = cornerBase; currentOutlineRenderer = outlineBase; }
            else { currentCornerRenderer = cornerAlt; currentOutlineRenderer = outlineAlt; }

            ToggleTypeVisuals(isBaseType);
            MirrorTypeVisuals(isFlippedType);
        }

        void ToggleTypeVisuals(bool isBaseType)
        {
            cornerBase.gameObject.SetActive(isBaseType);
            outlineBase.gameObject.SetActive(isBaseType);
            cornerAlt.gameObject.SetActive(!isBaseType);
            outlineAlt.gameObject.SetActive(!isBaseType);

            SetOutlineState(false);
            ChangeOutlineSprite(currentOutlineRenderer);
            SetOutlineState(true);
        }

        void MirrorTypeVisuals(bool isFlippedType)
        {
            currentCornerRenderer.flipX = isFlippedType;
            currentOutlineRenderer.flipX = isFlippedType;
        }

        public override List<List<Vector2>> NodePathIndexesMapper()
        {
            return new List<List<Vector2>>
            {
                new List<Vector2> { nodePositions[0].position, nodePositions[1].position },
                new List<Vector2> { nodePositions[1].position, nodePositions[0].position },
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
