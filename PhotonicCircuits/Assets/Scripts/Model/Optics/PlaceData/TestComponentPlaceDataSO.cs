using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "testPlaceData", menuName = "ScriptableObjects/Components/Place Data/test")]
    public class TestComponentPlaceDataSO : ComponentPlaceDataSO
    {
        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy, Orientation placeOrientation)
        {
            return new TestComponent(
                hostGrid,
                tilesToOccupy,
                placeOrientation,
                inPorts,
                outPorts);
        }
    }
}
