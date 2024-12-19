using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ICOutComponentPlaceData", menuName = "ScriptableObjects/Components/Place Data/IC/Out Component")]
    public class ICOutComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy, Orientation placeOrientation)
        {
            return new ICOutComponent(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                placeOrientation,
                inPorts,
                outPorts);
        }
    }
}
