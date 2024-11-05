using Game.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class OpticComponent
    {
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

            this.inPorts = inPorts;
            this.outPorts = outPorts;

            CompilePorts(this.inPorts);
            CompilePorts(this.outPorts);
        }

        private void CompilePorts(ComponentPort[] ports)
        {
            foreach (ComponentPort port in ports)
                port.owner = this;
        }

        protected virtual Vector2Int GetOccupiedRootTile(Vector2Int[] tilesToOccupy)
        {
            return tilesToOccupy[0];
        }
    }
}
