using UnityEngine;

namespace Game.Data
{
    public class ComponentPort
    {
        [HideInInspector] public OpticComponent owner;
        public Vector2Int position;
        public Orientation orientation;
    }
}
