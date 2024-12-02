using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "sourceLaserPlaceData", menuName = "ScriptableObjects/Components/Place Data/Laser source")]
    public class PhotonLaserSourcePlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return new PhotonLaserSourceComponent(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts);
        }
    }
}
