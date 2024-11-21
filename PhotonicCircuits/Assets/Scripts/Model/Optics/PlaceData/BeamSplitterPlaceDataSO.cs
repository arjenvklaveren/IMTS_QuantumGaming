using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "beamSplitterPlaceData", menuName = "ScriptableObjects/Components/Place Data/beamSplitter")]
    public class BeamSplitterPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return new BeamSplitterComponent(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts);
        }
    }
}