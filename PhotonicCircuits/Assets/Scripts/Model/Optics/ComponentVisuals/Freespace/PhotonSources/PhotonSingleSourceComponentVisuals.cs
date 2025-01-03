using Game.Data;
using SadUtils;
using UnityEngine;

namespace Game
{
    public class PhotonSingleSourceComponentVisuals : ComponentVisuals
    {
        [Header("Photon Visuals Settings")]
        [SerializeField] private PhotonParticleVisuals photonPrefab;

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
            PhotonSingleSourceComponent source = SourceComponent as PhotonSingleSourceComponent;
            source.OnCreatePhoton += Source_OnCreatePhoton;
        }

        private void RemoveListeners()
        {
            PhotonSingleSourceComponent source = SourceComponent as PhotonSingleSourceComponent;
            source.OnCreatePhoton -= Source_OnCreatePhoton;
        }
        #endregion

        #region Handle Events
        private void Source_OnCreatePhoton(Photon photon) => HandlePhotonCreation(photon);

        private void HandlePhotonCreation(Photon photon)
        {
            PhotonVisuals photonVisuals = Instantiate(photonPrefab);

            photonVisuals.SetSource(photon);
            photonVisuals.SyncVisuals();
            photonVisuals.StartMovement();
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
