using Game.Data;
using UnityEngine;

namespace Game
{
    public class ICComponent2x2 : ICComponentBase
    {
        public override OpticComponentType Type => OpticComponentType.IC2x2;

        public ICComponent2x2(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            Vector2Int internalGridSize,
            Vector2 internalGridSpacing,
            string title) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                internalGridSize,
                internalGridSpacing,
                title)
        {
        }

        public ICComponent2x2(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            ICBlueprintData blueprintData) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                blueprintData)
        {
        }
    }
}
