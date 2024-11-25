using Game.Data;
using UnityEngine;

namespace Game
{
    public class BeamSplitterComponentVisuals : ComponentVisuals
    {
        [Header("Photon Visuals Settings")]
        [SerializeField] private PhotonVisuals photonPrefab;

        private BeamSplitterComponent sourceSplitter;

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
            sourceSplitter = SourceComponent as BeamSplitterComponent;
        }

        private void SetupListeners()
        {
            sourceSplitter.OnSplitPhoton += Source_OnSplitPhoton;
        }

        private void RemoveListeners()
        {
            sourceSplitter.OnSplitPhoton -= Source_OnSplitPhoton;
        }
        #endregion

        #region Handle Events
        private void Source_OnSplitPhoton(Photon photon1, Photon photon2) => HandlePhotonCreation(photon1, photon2);

        private void HandlePhotonCreation(params Photon[] photons)
        {
            foreach (Photon photon in photons)
                CreatePhotonVisuals(photon);
        }

        private void CreatePhotonVisuals(Photon photon)
        {
            PhotonVisuals photonVisuals = Instantiate(photonPrefab);

            photonVisuals.SetSource(photon);
            photonVisuals.SyncVisuals();
            photonVisuals.StartMovement();
        }
        #endregion

        protected override void HandlePhoton(PhotonVisuals photon)
        {
            // Force move photon visuals to center of component.
            Vector2 photonStartPos = photon.transform.position;
            Vector2 photonEndPos = GridUtils.GridPos2WorldPos(SourceComponent.occupiedRootTile, SourceComponent.HostGrid);

            photon.ForceMoveHalfTile(photonStartPos, photonEndPos);
        }
    }
}
