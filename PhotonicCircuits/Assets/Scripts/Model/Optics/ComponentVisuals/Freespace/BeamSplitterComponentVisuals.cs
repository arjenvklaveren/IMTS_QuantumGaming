using Game.Data;
using UnityEngine;

namespace Game
{
    public class BeamSplitterComponentVisuals : ComponentVisuals
    {
        [SerializeField] private PhotonParticleVisuals photonParticlePrefab;
        [SerializeField] private PhotonBeamVisuals photonBeamPrefab;

        private PhotonVisuals photonPrefab;
        private BeamSplitterComponent sourceSplitter;

        private PhotonBeamVisuals tempBeamVisuals;

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
            for(int i = 0; i <  photons.Length; i++)
            {
                CreatePhotonVisuals(photons[i], i);
            }                
        }

        private void CreatePhotonVisuals(Photon photon, int index)
        {
            PhotonVisuals photonVisuals = null;

            if(index == 0 && photonPrefab is PhotonBeamVisuals)
            {
                PhotonBeamVisuals beamVisuals = Instantiate(tempBeamVisuals);
                beamVisuals.SetSource(photon);
            }
            else
            {
                photonVisuals = Instantiate(photonPrefab);
                photonVisuals.SetSource(photon);
            }
        }
        #endregion

        protected override void HandlePhoton(PhotonVisuals photon)
        {
            if (photon is PhotonParticleVisuals)
            {
                photonPrefab = photonParticlePrefab;
                PhotonParticleVisuals photonParticle = photon as PhotonParticleVisuals;

                // Force move photon visuals to center of component.
                Vector2 photonStartPos = photon.transform.position;
                Vector2 photonEndPos = GridUtils.GridPos2WorldPos(SourceComponent.occupiedRootTile, SourceComponent.HostGrid);

                photonParticle.ForceMoveHalfTile(photonStartPos, photonEndPos);
            }
            else
            {
                photonPrefab = photonBeamPrefab;
                PhotonBeamVisuals photonBeam = photon as PhotonBeamVisuals;
                tempBeamVisuals = photonBeam;
            }
        }
    }
}
