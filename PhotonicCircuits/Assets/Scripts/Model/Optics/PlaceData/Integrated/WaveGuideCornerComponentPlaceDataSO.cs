using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "waveGuideCornerPlaceData", menuName = "ScriptableObjects/Components/Place Data/waveguide corner")]
    public class WaveGuideCornerComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy, Orientation placeOrientation)
        {
            return new WaveGuideCornerComponent(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                placeOrientation,
                inPorts,
                outPorts);
        }
    }
}
