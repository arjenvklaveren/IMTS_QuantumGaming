using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "phaseShifterPlaceData", menuName = "ScriptableObjects/Components/Place Data/phaseShifter")]
    public class PhaseShifterComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(Vector2Int[] tilesToOccupy)
        {
            return new PhaseShifterComponent(
                tilesToOccupy,
                inPorts,
                outPorts);
        }
    }
}
