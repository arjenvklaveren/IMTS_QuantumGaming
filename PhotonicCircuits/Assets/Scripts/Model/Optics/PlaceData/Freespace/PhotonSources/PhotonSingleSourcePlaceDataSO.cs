using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "sourceSinglePlaceData", menuName = "ScriptableObjects/Components/Place Data/Single source")]
    public class PhotonSingleSourcePlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return new PhotonSingleSourceComponent(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts);
        }
    }
}
