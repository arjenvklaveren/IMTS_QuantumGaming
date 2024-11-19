using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "sourcePlaceData", menuName = "ScriptableObjects/Components/Place Data/source")]
    public class PhotonSourcePlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(Vector2Int[] tilesToOccupy)
        {
            return new PhotonSourceComponent(
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts);
        }
    }
}
