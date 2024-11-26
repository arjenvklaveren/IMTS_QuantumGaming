using UnityEngine;

namespace Game.Data
{
    public static class GridUtils
    {
        #region Position Conversion
        public static Vector2 GridPos2WorldPos(Vector2Int gridPos, GridData grid)
        {
            return GridPos2WorldPos(gridPos, grid.spacing);
        }

        public static Vector2 GridPos2WorldPos(Vector2Int gridPos, Vector2 gridSpacing)
        {
            return gridPos * gridSpacing;
        }

        public static Vector2Int GridPos2ChunkPos(Vector2Int gridPos)
        {
            Vector2 gridPosFloat = gridPos;

            return new(
                Mathf.FloorToInt(gridPosFloat.x / GridData.CHUNK_SIZE),
                Mathf.FloorToInt(gridPosFloat.y / GridData.CHUNK_SIZE));
        }
        #endregion

        #region Bounds Check
        public static bool IsPosOutOfBounds(Vector2Int pos, GridData grid)
        {
            return pos.x < 0 ||
                pos.y < 0 ||
                pos.x >= grid.size.x ||
                pos.y >= grid.size.y;
        }

        public static bool IsChunkInBounds(Vector2Int chunkPos, GridData grid)
        {
            if (chunkPos.x < 0 || chunkPos.y < 0)
                return false;

            return chunkPos.x < grid.ChunksCount.x &&
                chunkPos.y < grid.ChunksCount.y;
        }
        #endregion
    }
}
