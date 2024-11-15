using Game.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class BeamSplitterComponentVisuals : ComponentVisuals
    {
        [Header("Photon Visuals Settings")]
        [SerializeField] private PhotonVisuals photonPrefab;

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
            BeamSplitterComponent source = SourceComponent as BeamSplitterComponent;
            source.OnSplitPhoton += Source_OnSplitPhoton;
        }

        private void RemoveListeners()
        {
            BeamSplitterComponent source = SourceComponent as BeamSplitterComponent;
            source.OnSplitPhoton -= Source_OnSplitPhoton;
        }
        #endregion

        #region Handle Events
        private void Source_OnSplitPhoton(Photon photon1, Photon photon2) => HandlePhotonCreation(photon1, photon2);

        private void HandlePhotonCreation(Photon photon1, Photon photon2)
        {
            PhotonVisuals photon1Visuals = Instantiate(photonPrefab);
            PhotonVisuals photon2Visuals = Instantiate(photonPrefab);

            photon1Visuals.SetSource(photon1);
            photon2Visuals.SetSource(photon2);
            photon1Visuals.SyncVisuals();
            photon2Visuals.SyncVisuals();
        }
        #endregion
    }
}
