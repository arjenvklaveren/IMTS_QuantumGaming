using Game.Data;
using UnityEngine;

namespace Game
{
    public class GridGenerator : MonoBehaviour
    {
        [SerializeField] private GridTile tilePrefab;
        [SerializeField] private Transform gridHolder;

        public GridGenerationData generationSettings;

        #region Clear
        public void ClearGrid()
        {
            for (int i = gridHolder.childCount - 1; i >= 0; i--)
                Destroy(gridHolder.GetChild(i).gameObject);
        }
        #endregion

        #region Generation
        public void GenerateGrid()
        {
            if (gridHolder.childCount > 0)
                ClearGrid();

            for (int y = 0; y < generationSettings.size.y; y++)
                GenerateGridRow(y);
        }

        private void GenerateGridRow(int yPos)
        {
            for (int x = 0; x < generationSettings.size.x; x++)
                GenerateTile(x, yPos);
        }

        private void GenerateTile(int xPos, int yPos)
        {
            Vector2Int position = new(xPos, yPos);

            GridTile spawnedTile = Instantiate(tilePrefab, (Vector2)position, Quaternion.identity, gridHolder);
            spawnedTile.position = position;
        }
        #endregion
    }
}
