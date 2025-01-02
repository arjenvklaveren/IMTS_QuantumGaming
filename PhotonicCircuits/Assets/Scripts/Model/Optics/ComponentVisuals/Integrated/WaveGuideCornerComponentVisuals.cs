using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class WaveGuideCornerComponentVisuals : WaveGuideComponentVisuals
    {
        [SerializeField] GameObject cornerBase;
        [SerializeField] GameObject cornerAlt;

        private WaveGuideCornerComponent sourceCorner;

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
            sourceCorner.OnChangeAltType += Source_OnChangeAltType;
        }

        private void RemoveListeners()
        {
            sourceCorner.OnChangeAltType -= Source_OnChangeAltType;
        }
        #endregion

        private void Source_OnChangeAltType(bool isAlt)
        {
            cornerBase.SetActive(!isAlt);
            cornerAlt.SetActive(isAlt);
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
