using Game.Data;
using UnityEngine;

namespace Game
{
    public static class PhotonPathFinder
    {
        #region Find In Port
        public static bool TryFindNextInPort(
            Vector2Int startPosition,
            Orientation photonOrientation,
            GridData grid,
            out ComponentPort port)
        {
            port = null;
            Orientation desiredOrientation = photonOrientation.RotateClockwise(2);
            Vector2Int chunkToSearch = GridUtils.GridPos2ChunkPos(startPosition);

            while (true)
            {
                if (!GridUtils.IsChunkInBounds(chunkToSearch, grid))
                    return false;

                if (grid.inPortsData.ContainsKey(chunkToSearch))
                    if (TrySearchChunk(chunkToSearch, desiredOrientation, grid, out port))
                        return true;

                chunkToSearch += photonOrientation.ToVector2Int();
            }
        }

        private static bool TrySearchChunk(
            Vector2Int chunkPos,
            Orientation desiredOrientation,
            GridData grid,
            out ComponentPort port)
        {
            ChunkPortData chunkPortData = grid.inPortsData[chunkPos];

            return chunkPortData.TryFindClosestPort(
                chunkPos,
                desiredOrientation,
                out port);
        }
        #endregion
    }
}
