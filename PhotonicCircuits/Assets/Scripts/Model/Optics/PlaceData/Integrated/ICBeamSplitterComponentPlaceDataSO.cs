using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "integratedBeamSplitterPlaceData", menuName = "ScriptableObjects/Components/Place Data/integrated beamSplitter")]
    public class ICBeamSplitterComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy, Orientation placeOrientation)
        {
            return new ICBeamSplitterComponent(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                placeOrientation,
                inPorts,
                outPorts);
        }
    }
}
