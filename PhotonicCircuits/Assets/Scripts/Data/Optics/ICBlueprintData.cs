namespace Game.Data
{
    public class ICBlueprintData
    {
        public OpticComponentType type;

        public GridData internalGrid;

        public string Name => internalGrid.gridName;
    }
}
