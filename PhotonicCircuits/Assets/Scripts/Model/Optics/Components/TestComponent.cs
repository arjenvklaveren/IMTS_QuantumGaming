using Game.Data;
using UnityEngine;

namespace Game
{
    public class TestComponent : OpticComponent
    {
        public override OpticComponentType Type => OpticComponentType.Test;

        public TestComponent(
            Vector2Int[] tilesToOccupy,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts
            ) : base(
                tilesToOccupy,
                inPorts,
                outPorts)
        {
        }
    }
}
