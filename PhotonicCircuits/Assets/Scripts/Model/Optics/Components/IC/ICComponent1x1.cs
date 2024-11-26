using Game.Data;
using UnityEngine;

namespace Game
{
    public class ICComponent1x1 : ICComponentBase
    {
        public override OpticComponentType Type => OpticComponentType.IC1x1;

        public ICComponent1x1(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts,
            Vector2Int internalGridSize,
            Vector2 internalGridSpacing,
            string title) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts,
                internalGridSize,
                internalGridSpacing,
                title)
        {
        }
    }
}
