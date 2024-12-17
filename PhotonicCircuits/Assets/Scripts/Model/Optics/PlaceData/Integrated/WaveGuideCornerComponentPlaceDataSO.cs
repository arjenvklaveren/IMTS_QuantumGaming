using Game.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "waveGuideCornerPlaceData", menuName = "ScriptableObjects/Components/Place Data/waveguide corner")]
    public class WaveGuideCornerComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return new WaveGuideCornerComponent(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts);
        }
    }
}
