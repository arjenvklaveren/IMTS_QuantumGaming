using Game.Data;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "ICPlaceData", menuName = "ScriptableObjects/Components/Place Data/IC/Integrated Circuit")]
    public class ICComponentPlaceDataSO : ComponentPlaceDataSO
    {
        [Header("IC Settings")]
        [SerializeField] private OpticComponentType ICType;

        [Header("Internal Grid Settings")]
        [SerializeField] private Vector2Int gridSize;
        [SerializeField] private Vector2 gridSpacing;

        public override OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy, Orientation placeOrientation)
        {
            return new ICComponentBase(
                hostGrid,
                tilesToOccupy,
                defaultOrientation,
                gridSize,
                gridSpacing,
                "",
                ICType);
        }
    }
}
