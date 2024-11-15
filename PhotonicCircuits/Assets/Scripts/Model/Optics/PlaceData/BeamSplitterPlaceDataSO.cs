using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "beamSplitterPlaceData", menuName = "ScriptableObjects/Components/Place Data/beam splitter")]
    public class BeamSplitterPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(Vector2Int[] tilesToOccupy)
        {
            return new BeamSplitterComponent(
                tilesToOccupy,
                inPorts,
                outPorts);
        }
    }
}