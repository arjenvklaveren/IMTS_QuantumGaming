using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "beamSplitterPlaceData", menuName = "ScriptableObjects/Components/Place Data/beamSplitter")]
    public class BeamSplitterPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy, Orientation placeOrientation)
        {
            return new BeamSplitterComponent(
                hostGrid,
                tilesToOccupy,
                placeOrientation,
                inPorts,
                outPorts);
        }
    }
}