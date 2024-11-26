using Game.Data;
using System;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ICPlaceData", menuName = "ScriptableObjects/Components/Place Data/Integrated Circuit")]
    public class ICComponentPlaceDataSO : ComponentPlaceDataSO
    {
        [Header("IC Settings")]
        [SerializeField] private OpticComponentType ICType;

        [Header("Internal Grid Settings")]
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private Vector2 gridSpacing;
        public string fileName;

        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return ICType switch
            {
                OpticComponentType.IC1x1 => Create1x1(hostGrid, tilesToOccupy),
                OpticComponentType.IC2x2 => Create2x2(hostGrid, tilesToOccupy),
                _ => throw new NotImplementedException($"Place Data for IC Component of type {ICType} has not been implemented!")
            };
        }

        #region Create IC Components
        private ICComponent1x1 Create1x1(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return new(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts,
                gridSize,
                gridSpacing,
                fileName);
        }

        private ICComponent2x2 Create2x2(GridData hostGrid, Vector2Int[] tilesToOccupy)
        {
            return new(
                hostGrid,
                tilesToOccupy,
                orientation,
                inPorts,
                outPorts,
                gridSize,
                gridSpacing,
                fileName);
        }
        #endregion
    }
}
