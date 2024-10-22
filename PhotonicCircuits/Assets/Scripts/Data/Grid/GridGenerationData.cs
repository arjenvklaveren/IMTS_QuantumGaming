using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "GridData", menuName = "ScriptableObjects/Grid/GridData")]
    public class GridGenerationData : ScriptableObject
    {
        public Vector2 spacing;
        public Vector2Int size;
    }
}
