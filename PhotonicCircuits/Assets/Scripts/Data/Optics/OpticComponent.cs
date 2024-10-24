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

        public OpticComponent(Vector2Int[] tilesToOccupy)
        {
            occupiedTiles = new(tilesToOccupy);
            occupiedRootTile = tilesToOccupy[0];
        }
    }
}
