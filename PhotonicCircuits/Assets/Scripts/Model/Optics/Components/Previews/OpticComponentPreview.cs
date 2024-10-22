using UnityEngine;

namespace Game
{
    public abstract class OpticComponentPreview : ScriptableObject
    {
        public OpticComponent componentPrefab;

        public virtual Vector2Int[] GetTilesToOccupy(Vector2Int topLeftCorner)
        {
            return new Vector2Int[1] { topLeftCorner };
        }
    }
}
