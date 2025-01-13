using Game.Data;
using SadUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MeasuringManager : Singleton<MeasuringManager>
    {
        public Action<Photon, int> OnMeasurePhoton;

        protected override void Awake()
        {
            SetInstance(this);
        }

        public void MeasurePhoton(PhotonDetectorComponent detector, Photon photon)
        {
            OnMeasurePhoton?.Invoke(photon, detector.GetStateIdentifier());
        }
    }
}
