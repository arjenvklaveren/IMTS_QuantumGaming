using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "photonDetectorPlaceData", menuName = "ScriptableObjects/Components/Place Data/photonDetector")]
    public class PhotonDetectorComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(Vector2Int[] tilesToOccupy)
        {
            return new PhotonDetectorComponent(
                tilesToOccupy,
                inPorts,
                outPorts);
        }
    }
}
