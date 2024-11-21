using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "sourcePlaceData", menuName = "ScriptableObjects/Components/Place Data/source")]
    public class PhotonSourcePlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return new PhotonSourceComponent(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts);
        }
    }
}
