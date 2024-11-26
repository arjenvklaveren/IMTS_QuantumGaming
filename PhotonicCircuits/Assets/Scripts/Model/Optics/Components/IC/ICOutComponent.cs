using Game.Data;
using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class ICOutComponent : OpticComponent
    {
        public event Action<Photon> OnDetectPhoton;

        public override OpticComponentType Type => OpticComponentType.ICOut;

        public int portId;

        public ICOutComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts)
        {
        }

        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            OnDetectPhoton?.Invoke(photon);

            yield break;
        }
    }
}
