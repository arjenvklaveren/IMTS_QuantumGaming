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
        [SerializeField] Transform pathNodeTransform;

        private WaveGuideCornerComponent sourceCorner;

        private SpriteRenderer currentCornerRenderer;
        private SpriteRenderer currentOutlineRenderer;

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
