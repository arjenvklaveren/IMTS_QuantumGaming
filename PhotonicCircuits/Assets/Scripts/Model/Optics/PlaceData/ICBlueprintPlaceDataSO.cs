using Game.Data;
using System;
using UnityEngine;

namespace Game
{
    public class ICBlueprintPlaceDataSO : ComponentPlaceDataSO
    {
        private ICBlueprintData blueprintData;

        public void SetBlueprintReference(ComponentPlaceDataSO templatePlaceData, ICBlueprintData blueprintData)
        {
            // Load blueprint Data
            //ICBlueprintManager.Instance.TryGetBlueprintData(blueprintName, out blueprintData);
            this.blueprintData = blueprintData;

            // Set place data from template
            tileOffsetsToOccupy = templatePlaceData.tileOffsetsToOccupy;
        }

        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return blueprintData.type switch
            {
                OpticComponentType.IC1x1 => Get1x1(hostGrid, tilesToOccupy),
                OpticComponentType.IC2x2 => Get2x2(hostGrid, tilesToOccupy),
                _ => throw new NotImplementedException($"Place data creation for type \"{blueprintData.type}\" has not been implemented.")
            };
        }

        #region Create Optic Components
        private ICComponent1x1 Get1x1(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return new(
                hostGrid,
                tilesToOccupy,
                Orientation.Up,
                blueprintData);
        }

        private ICComponent2x2 Get2x2(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return new(
                hostGrid,
                tilesToOccupy,
                Orientation.Up,
                blueprintData);
        }
        #endregion
    }
}
