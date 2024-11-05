using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "testPlaceData", menuName = "ScriptableObjects/Components/Place Data/test")]
    public class TestComponentPlaceDataSO : ComponentPlaceDataSO
    {
        [SerializeField] private ComponentPort[] inPorts;
        [SerializeField] private ComponentPort[] outPorts;

        public override OpticComponent CreateOpticComponent(Vector2Int[] tilesToOccupy)
        {
            return new TestComponent(
                tilesToOccupy,
                inPorts,
                outPorts);
        }
    }
}
