using Game.Data;
using UnityEngine;

namespace Game
{
    public class ICBlueprintPlaceDataSO : ComponentPlaceDataSO
    {
        private ICBlueprintData blueprintData;

        public void SetBlueprintReference(ComponentPlaceDataSO templatePlaceData, ICBlueprintData blueprintData)
        {
            // Store blueprint Data
            this.blueprintData = blueprintData;

            // Set place data from template
            tileOffsetsToOccupy = templatePlaceData.tileOffsetsToOccupy;
        }

        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy, Orientation placeOrientation)
        {
            return new ICComponentBase(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                blueprintData);
        }
    }
}
