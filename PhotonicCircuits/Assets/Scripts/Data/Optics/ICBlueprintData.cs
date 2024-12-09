namespace Game.Data
{
    public class ICBlueprintData
    {
        public string Name => internalGrid.gridName;

        public GridData internalGrid;
        public OpticComponentType type;

        public ICBlueprintData(GridData grid, OpticComponentType type)
        {
            internalGrid = grid;
            this.type = type;
        }
    }
}
