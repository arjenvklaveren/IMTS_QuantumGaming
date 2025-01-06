using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "integratedPhaseShifterPlaceData", menuName = "ScriptableObjects/Components/Place Data/integrated phaseShifter")]
    public class ICPhaseShifterComponentPlaceDataSO : ComponentPlaceDataSO
    {
        [SerializeField] private float shiftAmountRadians;
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy, Orientation placeOrientation)
        {
            return new ICPhaseShifterComponent(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                placeOrientation,
                inPorts,
                outPorts,
                shiftAmountRadians);
        }
    }
}
