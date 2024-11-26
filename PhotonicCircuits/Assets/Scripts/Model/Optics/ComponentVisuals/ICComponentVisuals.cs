using Game.Data;
using UnityEngine;

namespace Game
{
    public class ICComponentVisuals : ComponentVisuals
    {
        [SerializeField] private Transform visualsHolder;

        [Space]
        [SerializeField] private PhotonVisuals photonPrefab;

        private ICComponentBase sourceICComponent;

        #region Create / Destroy
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

        private void SetDefaultValues()
        {
            sourceICComponent = SourceComponent as ICComponentBase;
        }

        private void SetupListeners()
        {
            sourceICComponent.OnPhotonExitIC += ICComponentBase_OnPhotonExitIC;
        }

        private void RemoveListeners()
        {
            sourceICComponent.OnPhotonExitIC -= ICComponentBase_OnPhotonExitIC;
        }
        #endregion

        #region Handle Events
        private void ICComponentBase_OnPhotonExitIC(Photon photon)
        {
            PhotonVisuals photonVisuals = Instantiate(photonPrefab);

            photonVisuals.SetSource(photon);
            photonVisuals.SyncVisuals();
            photonVisuals.StartMovement();
        }
        #endregion

        #region Handle Photon
        protected override void HandlePhoton(PhotonVisuals photon)
        {
            Destroy(photon.gameObject);
        }
        #endregion

        #region Handle Rotation
        protected override void HandleRotationChanged(Orientation orientation) => RotateToLookAtOrientation(visualsHolder, orientation);
        #endregion

        #region Handle Interaction
        public void Interact()
        {
            GridManager.Instance.OpenGrid(sourceICComponent.internalGrid);
        }
        #endregion
    }
}
