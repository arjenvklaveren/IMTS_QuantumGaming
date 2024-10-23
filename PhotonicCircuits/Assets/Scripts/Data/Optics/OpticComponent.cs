using Game.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class OpticComponent
    {
        public abstract OpticComponentType ComponentType { get; }

        public readonly HashSet<Vector2Int> occupiedTiles;

        public OpticComponent(Vector2Int[] tilesToOccupy)
        {
            occupiedTiles = new(tilesToOccupy);
        }
    }
}
