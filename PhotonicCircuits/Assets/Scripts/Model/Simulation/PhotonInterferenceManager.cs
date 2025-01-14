using SadUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Data;

namespace Game
{
    public enum InterferenceType
    {
        Combine,
        Split,
        Identical,
        Entangled
    }
    public class PhotonInterferenceManager : Singleton<PhotonInterferenceManager>
    {
        private struct InterferenceData
        {
            public Photon photonA, photonB;
            public InterferenceType type;
        }
        protected override void Awake()
        {
            SetInstance(this);
        }
        public void HandleInterference(Photon photonA, Photon photonB, InterferenceType type)
        {
            InterferenceData data = new InterferenceData()
            {
                photonA = photonA,
                photonB = photonB,
                type = type
            };

            switch (type)
            {
                case InterferenceType.Split:
                    HandleSplitInterference(data);
                    break;
                case InterferenceType.Combine:
                    HandleCombineInterference(data);
                    break;
                case InterferenceType.Identical:
                    HandleIdenticalInterference(data);
                    break;
                case InterferenceType.Entangled:
                    HandleEntangledInterference(data);
                    break;
                default:
                    break;
            }
        }
        void HandleSplitInterference(InterferenceData data)
        {
            float combinedProbabilities = data.photonA.GetAmplitude() + data.photonB.GetAmplitude();
            float phaseDiff = Mathf.Abs(data.photonA.GetPhase() - data.photonB.GetPhase()) % Mathf.PI;
            float phaseDiffNormalisedSlope = (phaseDiff <= Mathf.PI / 2) ? phaseDiff / (Mathf.PI / 2) : (Mathf.PI - phaseDiff) / (Mathf.PI / 2);

            float photonProbabiltyB = combinedProbabilities * phaseDiffNormalisedSlope;
            float photonProbabiltyA = combinedProbabilities - photonProbabiltyB;

            data.photonA.SetAmplitude(photonProbabiltyA);
            data.photonB.SetAmplitude(photonProbabiltyB);

            if(photonProbabiltyA == 0) PhotonManager.Instance.RemovePhoton(data.photonA, false);
            if(photonProbabiltyB == 0) PhotonManager.Instance.RemovePhoton(data.photonB, false);
        }

        void HandleCombineInterference(InterferenceData data)
        {

        }

        void HandleIdenticalInterference(InterferenceData data)
        {

        }

        void HandleEntangledInterference(InterferenceData data)
        {

        }

        public bool IsInterfering(Photon photonA, Photon photonB, bool checkPropagation = true)
        {
            bool result = PhotonManager.Instance.GetPhotonSuperpositions(photonA).Contains(photonB) &&
                photonA.IsOfSameType(photonB);

            if (checkPropagation && result) result = photonA.GetPropagation() == photonB.GetPropagation();

            return result;
        }
    }
}
