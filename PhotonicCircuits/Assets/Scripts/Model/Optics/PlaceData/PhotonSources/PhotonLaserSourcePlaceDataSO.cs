using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "sourceLaserPlaceData", menuName = "ScriptableObjects/Components/Place Data/Laser source")]
    public class PhotonLaserSourcePlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy, Orientation placeOrientation)
        {
            return new PhotonLaserSourceComponent(
                hostGrid,
                tilesToOccupy,
                placeOrientation,
                inPorts,
                outPorts);
        }
    }
}
