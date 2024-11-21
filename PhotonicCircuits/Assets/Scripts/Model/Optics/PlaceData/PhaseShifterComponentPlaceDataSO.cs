using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "phaseShifterPlaceData", menuName = "ScriptableObjects/Components/Place Data/phaseShifter")]
    public class PhaseShifterComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return new PhaseShifterComponent(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts);
        }
    }
}
