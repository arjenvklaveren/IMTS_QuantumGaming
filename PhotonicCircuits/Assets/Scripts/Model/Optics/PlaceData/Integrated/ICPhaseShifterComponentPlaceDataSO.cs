using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "integratedPhaseShifterPlaceData", menuName = "ScriptableObjects/Components/Place Data/integrated phaseShifter")]
    public class ICPhaseShifterComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return new ICPhaseShifterComponent(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts);
        }
    }
}
