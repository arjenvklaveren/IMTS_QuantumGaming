using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ICOutComponentPlaceData", menuName = "ScriptableObjects/Components/Place Data/IC/Out Component")]
    public class ICOutComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return new ICOutComponent(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts);
        }
    }
}
