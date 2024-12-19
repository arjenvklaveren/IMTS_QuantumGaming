using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "integratedPhaseShifterPlaceData", menuName = "ScriptableObjects/Components/Place Data/integrated phaseShifter")]
    public class ICPhaseShifterComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy, Orientation placeOrientation)
        {
            return new ICPhaseShifterComponent(
                hostGrid,
                tilesToOccupy,
                placeOrientation,
                inPorts,
                outPorts);
        }
    }
}
