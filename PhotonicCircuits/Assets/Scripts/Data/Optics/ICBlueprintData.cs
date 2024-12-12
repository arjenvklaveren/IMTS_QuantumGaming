using System.Collections.Generic;

namespace Game.Data
{
    public class ICBlueprintData
    {
        public string Name => internalGrid.gridName;

        public Dictionary<string, int> containedBlueprints;

        public GridData internalGrid;
        public OpticComponentType type;

        public ICBlueprintData(
            Dictionary<string, int> containedBlueprints,
            GridData grid,
            OpticComponentType type)
        {
            this.containedBlueprints = new(containedBlueprints);

            internalGrid = grid;
            this.type = type;
        }
    }
}
