using UnityEngine;

namespace Game
{
    public abstract class OpticComponent : MonoBehaviour
    {
        public Vector3Int leftTopCoordinate;

        public virtual Vector3Int[] GetOccupiedTiles()
        {
            return new Vector3Int[1] { leftTopCoordinate };
        }
    }
}
