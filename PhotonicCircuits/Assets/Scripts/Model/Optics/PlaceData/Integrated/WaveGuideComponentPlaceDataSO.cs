using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "waveGuideStraightPlaceData", menuName = "ScriptableObjects/Components/Place Data/waveguide straight")]
    public class WaveGuideComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy, Orientation placeOrientation)
        {
            return new WaveGuideComponent(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                placeOrientation,
                inPorts,
                outPorts);
        }
    }
}
