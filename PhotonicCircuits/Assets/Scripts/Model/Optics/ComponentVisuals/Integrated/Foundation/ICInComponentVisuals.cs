using Game.Data;
using UnityEngine;

namespace Game
{
    public class ICInComponentVisuals : ComponentVisuals
    {
        [Header("Photon Settings")]
        [SerializeField] private PhotonVisuals photonPrefab;

        private ICInComponent sourceInComponent;

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

        private void SetDefaultValues()
        {
            sourceInComponent = SourceComponent as ICInComponent;
        }

        private void SetupListeners()
        {
            sourceInComponent.OnHandlePhoton += ICInComponent_OnHandlePhoton;
            SourceComponent.HostGrid.OnComponentRemoved += HostGrid_OnComponentRemoved;
        }

        private void RemoveListeners()
        {
            sourceInComponent.OnHandlePhoton -= ICInComponent_OnHandlePhoton;
            SourceComponent.HostGrid.OnComponentRemoved -= HostGrid_OnComponentRemoved;
        }
        #endregion

        #region Handle Events
        private void ICInComponent_OnHandlePhoton(Photon photon) => CreatePhotonVisuals(photon);
        private void HostGrid_OnComponentRemoved(OpticComponent component) => DestroySelfCheck(component);

        private void CreatePhotonVisuals(Photon photon)
        {
            PhotonVisuals photonVisuals = Instantiate(photonPrefab);

            photonVisuals.SetSource(photon);
        }

        private void DestroySelfCheck(OpticComponent component)
        {
            if (component != SourceComponent)
                return;

            Destroy(gameObject);
        }
        #endregion

        #region Rotation
        protected override void HandleRotationChanged(Orientation orientation) => RotateToLookAtOrientation(visualsHolder, orientation);
        #endregion
    }
}
