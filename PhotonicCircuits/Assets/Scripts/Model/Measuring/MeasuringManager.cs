using Game.Data;
using SadUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class MeasuringManager : Singleton<MeasuringManager>
    {
        protected override void Awake()
        {
            SetInstance(this);
        }

        public void MeasurePhoton(PhotonDetectorComponent detector, Photon photon)
        {

        }
    }
}
