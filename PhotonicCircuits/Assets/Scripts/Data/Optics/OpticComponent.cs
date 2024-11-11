using Game.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class OpticComponent
    {
        public static event Action<Photon> OnPhotonExit;

        public abstract OpticComponentType Type { get; }

        public readonly HashSet<Vector2Int> occupiedTiles;
        public readonly Vector2Int occupiedRootTile;

        public readonly ComponentPort[] inPorts;
        public readonly ComponentPort[] outPorts;

        public OpticComponent(
            Vector2Int[] tilesToOccupy,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts)
        {
            occupiedTiles = new(tilesToOccupy);
            occupiedRootTile = GetOccupiedRootTile(tilesToOccupy);

            inPorts ??= new ComponentPort[0];
            outPorts ??= new ComponentPort[0];

            this.inPorts = inPorts;
            this.outPorts = outPorts;

            CompilePorts(this.inPorts);
            CompilePorts(this.outPorts);
        }

        private void CompilePorts(ComponentPort[] ports)
        {
            int idCounter = 0;

            foreach (ComponentPort port in ports)
            {
                port.owner = this;
                port.portId = idCounter;

                port.position += occupiedRootTile;

                port.OnDetectPhoton += HandlePhoton;

                idCounter++;
            }
        }

        protected virtual Vector2Int GetOccupiedRootTile(Vector2Int[] tilesToOccupy)
        {
            return tilesToOccupy[0];
        }

        protected virtual void HandlePhoton(ComponentPort port, Photon photon) { }

        protected void TriggerOnPhotonExit(Photon photon)
        {
            OnPhotonExit?.Invoke(photon);
        }
    }
}
