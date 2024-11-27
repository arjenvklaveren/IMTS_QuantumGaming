using Game.Data;
using UnityEngine;

namespace Game
{
    public class PhotonSourceComponentVisuals : ComponentVisuals
    {
        [Header("Photon Visuals Settings")]
        [SerializeField] private PhotonVisuals photonPrefab;

        [Header("Visuals Holder")]
        [SerializeField] private Transform visualsHolder;

        #region Awake / Destroy
        public override void SetSource(OpticComponent component)
        {
            base.SetSource(component);

            SetupListeners();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveListeners();
        }

        private void SetupListeners()
        {
            PhotonSourceComponent source = SourceComponent as PhotonSourceComponent;
            source.OnCreatePhoton += PhotonSourceComponent_OnCreatePhoton;
        }

        private void RemoveListeners()
        {
            PhotonSourceComponent source = SourceComponent as PhotonSourceComponent;
            source.OnCreatePhoton -= PhotonSourceComponent_OnCreatePhoton;
        }
        #endregion

        #region Handle Events
        private void PhotonSourceComponent_OnCreatePhoton(Photon photon) => HandlePhotonCreation(photon);

        private void HandlePhotonCreation(Photon photon)
        {
            PhotonVisuals photonVisuals = Instantiate(photonPrefab);

            photonVisuals.SetSource(photon);
        }
        #endregion

        #region Handle Rotation
        protected override void HandleRotationChanged(Orientation orientation) => RotateToLookAtOrientation(visualsHolder, orientation);
        #endregion
    }
}
