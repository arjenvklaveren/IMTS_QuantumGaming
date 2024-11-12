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

        /// <summary>
        /// Constructor for creating a "ghost" port
        /// </summary>
        /// <param name="position">position of the port</param>
        /// <param name="orientation">orientation of the port</param>
        public ComponentPort(Vector2Int position, Orientation orientation)
        {
            this.position = position;
            this.orientation = orientation;
        }

        public ComponentPort(ComponentPort copy)
        {
            owner = copy.owner;
            portId = copy.portId;

            position = copy.position;
            orientation = copy.orientation;
        }

        public void ProcessPhoton(Photon photon)
        {
            if (owner == null)
            {
                // Destroy Photon here.
                Debug.Log("Destroying Photon...");
                return;
            }

            photon.TriggerEnterComponent(owner);

            OnDetectPhoton?.Invoke(this, photon);
        }
    }
}
