using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ICInComponentPlaceData", menuName = "ScriptableObjects/Components/Place Data/IC/In Component")]
    public class ICInComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy, Orientation placeOrientation)
        {
            return new ICInComponent(
                hostGrid,
                tilesToOccupy,
                placeOrientation,
                inPorts,
                outPorts);
        }
    }
}
