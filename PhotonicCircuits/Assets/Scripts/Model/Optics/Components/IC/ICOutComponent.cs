using Game.Data;
using System;
using System.Collections;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class ICOutComponent : OpticComponent
    {
        public event Action<Photon, int> OnDetectPhoton;

        public override OpticComponentType Type => OpticComponentType.ICOut;

        public int portId;

        public ICOutComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts,
            int portId = 0) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts)
        {
            this.portId = portId;
        }

        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            OnDetectPhoton?.Invoke(photon, portId);

            yield break;
        }

        public override string SerializeArgs()
        {
            return JsonConvert.SerializeObject(portId);
        }
    }
}
