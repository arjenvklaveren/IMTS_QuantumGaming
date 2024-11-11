using System;
using UnityEngine;

namespace Game.Data
{
    [System.Serializable]
    public class ComponentPort
    {
        public event Action<ComponentPort, Photon> OnDetectPhoton;

        [HideInInspector] public OpticComponent owner;
        [HideInInspector] public int portId;

        public Vector2Int position;
        public Orientation orientation;

        public void ProcessPhoton(Photon photon)
        {
            photon.TriggerEnterComponent(owner);

            OnDetectPhoton?.Invoke(this, photon);
        }
    }
}
