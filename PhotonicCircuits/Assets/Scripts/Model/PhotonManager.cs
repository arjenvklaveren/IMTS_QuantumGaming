using Game.Data;
using SadUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class PhotonManager : Singleton<PhotonManager>
    {
        [SerializeField] List<List<Photon>> photons;
        List<List<int>> entanglementIndexes;

        #region Awake / Destroy
        protected override void Awake()
        {
            SetDefaultValues();
            SetupListeners();

            SetInstance(this);
        }

        private void SetDefaultValues()
        {
            photons = new();
            entanglementIndexes = new();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void SetupListeners()
        {
            SimulationManager.OnSimulationStop += SimulationManager_OnSimulationStop;
        }

        private void RemoveListeners()
        {
            SimulationManager.OnSimulationStop -= SimulationManager_OnSimulationStop;
        }
        #endregion

        #region Handle Events
        private void SimulationManager_OnSimulationStop()
        {
            // Call Destroy Events.
            foreach (List<Photon> photonCollection in photons)
                foreach (Photon photon in photonCollection)
                    photon.Destroy();

            // Clear Lists
            photons.Clear();
            entanglementIndexes.Clear();
        }
        #endregion

        //Photon addition
        public void AddPhoton(Photon photon)
        {
            photons.Add(new List<Photon>() { photon });
        }

        public void AddEntangledPhotons(params Photon[] photons)
        {
            if (photons.Length < 2)
                return;

            List<int> newEntanglementList = new();

            foreach (Photon photon in photons)
            {
                int photonListIndex = this.photons.Count;
                this.photons.Add(new List<Photon> { photon });
                newEntanglementList.Add(photonListIndex);
            }
            entanglementIndexes.Add(newEntanglementList);
        }

        //Photon removing
        public void RemovePhoton(Photon photon, bool isMeasure)
        {
            Vector2Int? photonIndex = FindPhotonIndex2D(photon);
            if (!photonIndex.HasValue) return;

            if (isMeasure)
            {
                List<Photon> superpositions = GetPhotonSuperpositions(photon);
                List<Photon> entanglements = GetPhotonEntanglements(photon);

                foreach (Photon sPhoton in superpositions.ToList())
                {
                    DestroyPhoton(sPhoton, photonIndex.Value);
                }
                ShiftEntanglementIndexes(photonIndex.Value.x);

                foreach (Photon ePhoton in entanglements.ToList())
                {
                    //TODO photon entanglement logic
                }
            }
            else DestroyPhoton(photon, photonIndex.Value);
        }

        //Photon replacing
        public void ReplacePhoton(Photon photon, params Photon[] replacements)
        {
            Vector2Int? photonIndex = FindPhotonIndex2D(photon);
            if (!photonIndex.HasValue) return;
            photons[photonIndex.Value.x].RemoveAt(photonIndex.Value.y);
            photons[photonIndex.Value.x].InsertRange(photonIndex.Value.y, replacements);
            photon.Destroy();
        }

        void DestroyPhoton(Photon photon, Vector2Int photonIndex)
        {
            bool destroyVisuals = photon.GetPhotonType() == PhotonType.Quantum || !SimulationManager.Instance.IsSimulating();
            photon.Destroy(destroyVisuals);

            photons[photonIndex.x].RemoveAt(photonIndex.y);
            if (photons[photonIndex.x].Count == 0)
            {
                photons.RemoveAt(photonIndex.x);
                ShiftEntanglementIndexes(photonIndex.x);
            }
            else ReDistributeAmplitudeProbability(photon, photonIndex.x);
        }

        #region photon getters/finders
        public List<Photon> GetAllPhotons()
        {
            List<Photon> fullArray = new List<Photon>();
            for (int i = 0; i < photons.Count; i++)
            {
                fullArray.AddRange(photons[i]);
            }
            return fullArray;
        }
        public List<List<Photon>> GetAllPhotonsRaw() { return photons; }

        public List<Photon> GetPhotonSuperpositions(Photon photon)
        {
            int? listIndex = FindPhotonListIndex(photon);
            if (listIndex.HasValue) return photons[listIndex.Value];
            return new List<Photon>();
        }
        public List<Photon> GetPhotonEntanglements(Photon photon, bool includeSelf = true)
        {
            List<Photon> outPhotonList = new List<Photon>();
            int? photonIndex = FindPhotonListIndex(photon);
            int? entangleIndex = null;

            for (int i = 0; i < entanglementIndexes.Count; i++)
            {
                for (int j = 0; j < entanglementIndexes[i].Count; j++)
                {
                    if (entanglementIndexes[i][j] == photonIndex) { entangleIndex = i; break; }
                }
            }
            if (entangleIndex.HasValue)
            {
                for (int i = 0; i < entanglementIndexes[entangleIndex.Value].Count; i++)
                {
                    if (!includeSelf && entanglementIndexes[entangleIndex.Value][i] == photonIndex) continue;
                    outPhotonList.AddRange(photons[entanglementIndexes[entangleIndex.Value][i]]);
                }
            }
            return outPhotonList;
        }
        public List<Photon> GetAllLinkedOfPhoton(Photon photon)
        {
            List<Photon> outPhotonList = GetPhotonSuperpositions(photon);
            outPhotonList.AddRange(GetPhotonEntanglements(photon, false));
            return outPhotonList;
        }
        public List<Photon> GetPhotonsByIndex(int index)
        {
            return photons[index];
        }

        public int GetPhotonListCount() { return photons.Count; }

        //Photon finding
        public Photon FindPhoton(Photon photon)
        {
            foreach (List<Photon> photonList in photons)
            {
                foreach (Photon p in photonList)
                {
                    if (photon == p) return p;
                }
            }
            return null;
        }
        public int? FindPhotonListIndex(Photon photon)
        {
            Vector2Int? index2D = FindPhotonIndex2D(photon);
            if (index2D.HasValue) return index2D.Value.x;
            else return null;
        }
        public Vector2Int? FindPhotonIndex2D(Photon photon)
        {
            for (int i = 0; i < photons.Count; i++)
            {
                for (int j = 0; j < photons[i].Count; j++)
                {
                    if (photons[i][j] == photon) return new Vector2Int(i, j);
                }
            }
            return null;
        }
        #endregion

        void ShiftEntanglementIndexes(int cutoffIndex)
        {
            for (int i = 0; i < entanglementIndexes.Count; i++)
            {
                for (int j = 0; j < entanglementIndexes[i].Count; j++)
                {
                    if (entanglementIndexes[i][j] > cutoffIndex)
                    {
                        entanglementIndexes[i][j]--;
                    }
                }
            }
        }

        void ReDistributeAmplitudeProbability(Photon photon, int index)
        {
            if (photon.GetAmplitude() == 0) return;
            List<Photon> photonSuperpositions = GetPhotonsByIndex(index);
            float amplitudueDistributeVal = photon.GetAmplitude() / photonSuperpositions.Count;
            foreach(Photon pS in photonSuperpositions)
            {
                pS.SetAmplitude(pS.GetAmplitude() + amplitudueDistributeVal); 
            }
        }
    }
}
