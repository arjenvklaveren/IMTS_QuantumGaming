using Game.Data;
using SadUtils;
using UnityEngine;

namespace Game
{
    public class PhotonLaserSourceComponentVisuals : ComponentVisuals
    {
        [Header("Photon Visuals Settings")]
        [SerializeField] private PhotonBeamVisuals photonPrefab;

        [Header("Visuals Holder")]
        [SerializeField] private Transform visualsHolder;

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
            PhotonLaserSourceComponent source = SourceComponent as PhotonLaserSourceComponent;
            source.OnCreatePhoton += Source_OnCreatePhoton;
        }

        private void RemoveListeners()
        {
            PhotonLaserSourceComponent source = SourceComponent as PhotonLaserSourceComponent;
            source.OnCreatePhoton -= Source_OnCreatePhoton;
        }
        #endregion

        #region Handle Events
        private void Source_OnCreatePhoton(Photon photon) => HandlePhotonCreation(photon);

        private void HandlePhotonCreation(Photon photon)
        {
            PhotonBeamVisuals photonVisuals = Instantiate(photonPrefab);
            photonVisuals.transform.position = transform.position;
            photonVisuals.SetSource(photon);
        }
        #endregion

        #region Handle Rotation
        protected override void HandleRotationChanged(Orientation orientation)
        {
            RotateToLookAtOrientation(visualsHolder, orientation);
        }
        #endregion 
    }
}
