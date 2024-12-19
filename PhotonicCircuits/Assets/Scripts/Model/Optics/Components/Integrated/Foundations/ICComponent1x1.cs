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

        public ICComponent1x1(
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

        public override void SetOrientation(Orientation orientation) => ComponentRotateUtil.SetOrientation(this, orientation);
    }
}
