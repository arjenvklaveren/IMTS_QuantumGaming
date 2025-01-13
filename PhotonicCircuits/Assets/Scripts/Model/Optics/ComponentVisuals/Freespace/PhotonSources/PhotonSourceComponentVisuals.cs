using Game.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PhotonSourceComponentVisuals : ComponentVisuals
    {
        [Header("Photon Visuals Settings")]
        [SerializeField] protected PhotonVisuals photonPrefab;

        #region Awake / Destroy
        public override void SetSource(OpticComponent component)
        {
            base.SetSource(component);

            SetupListeners();
            HandleRotationChanged(component.orientation);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveListeners();
        }

        private void SetupListeners()
        {
            PhotonSourceComponent source = SourceComponent as PhotonSourceComponent;
            source.OnCreatePhoton += Source_OnCreatePhoton;
        }

        private void RemoveListeners()
        {
            PhotonSourceComponent source = SourceComponent as PhotonSourceComponent;
            source.OnCreatePhoton -= Source_OnCreatePhoton;
        }
        #endregion

        #region Handle Events
        private void Source_OnCreatePhoton(Photon photon) => HandlePhotonCreation(photon);
        protected virtual void HandlePhotonCreation(Photon photon) { }
        #endregion

        #region Handle Rotation
        protected override void HandleRotationChanged(Orientation orientation)
        {
            RotateToLookAtOrientation(visualsHolder, orientation);
        }
        #endregion 
    }
}
