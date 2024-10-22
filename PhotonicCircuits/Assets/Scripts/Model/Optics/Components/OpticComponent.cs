using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class OpticComponent : MonoBehaviour
    {
        public HashSet<Vector2Int> occupiedTiles;

        public abstract void ProcessPhoton(); // Should take a photon as input!

        public abstract string Serialize();
        public abstract void Deseialize(string json);
    }
}
