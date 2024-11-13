using Game.Data;
using UnityEngine;

namespace Game
{
    public static class PhotonPathFinder
    {
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

                if (TrySearchChunk(
                    startPosition,
                    chunkToSearch,
                    desiredOrientation,
                    grid,
                    out port))
                    return true;

                chunkToSearch += photonOrientation.ToVector2Int();
            }
        }

        // Can return a "ghost" port if obstacle is found first
        private static bool TrySearchChunk(
            Vector2Int searchPos,
            Vector2Int chunkPos,
            Orientation desiredOrientation,
            GridData grid,
            out ComponentPort port)
        {
            port = null;
            bool foundPort = false;

            if (grid.inPortsData.ContainsKey(chunkPos))
            {
                ChunkPortData chunkPortData = grid.inPortsData[chunkPos];

                foundPort = chunkPortData.TryFindClosestPort(
                    searchPos,
                    desiredOrientation,
                    out port);
            }

            bool foundObstacle = TryFindClosestObstacleInChunk(
                searchPos,
                chunkPos,
                desiredOrientation,
                grid,
                out ComponentPort ghostPort);

            // Figure out which port to output based on found + closest
            if (!foundPort && foundObstacle)
                port = ghostPort;

            if (foundPort && foundObstacle)
            {
                float distToPort = Vector2Int.Distance(port.position, searchPos);
                float distToGhostPort = Vector2Int.Distance(ghostPort.position, searchPos);

                if (distToPort > distToGhostPort)
                    port = ghostPort;
            }

            return foundPort || foundObstacle;
        }

        private static bool TryFindClosestObstacleInChunk(
            Vector2Int searchPos,
            Vector2Int chunkPos,
            Orientation desiredOrientation,
            GridData grid,
            out ComponentPort ghostPort)
        {
            ghostPort = null;

            Orientation searchOrientation = desiredOrientation.RotateClockwise(2); // Rotate 180 degrees.
            Vector2Int stepDir = searchOrientation.ToVector2Int();

            Vector2Int checkPos = GetStartSearchPosInChunk(
                searchPos,
                chunkPos,
                searchOrientation);

            // Search Chunk
            for (int i = 0; i < GridData.CHUNK_SIZE; i++)
            {
                if (!grid.occupiedTiles.Contains(checkPos))
                {
                    checkPos += stepDir;
                    continue;
                }

                // Found occupied Tile
                ghostPort = new(checkPos, desiredOrientation);
                return true;
            }

            return false;
        }

        private static Vector2Int GetStartSearchPosInChunk(
            Vector2Int searchPos,
            Vector2Int chunkPos,
            Orientation searchOrientation)
        {
            int chunkSize = GridData.CHUNK_SIZE;

            return searchOrientation switch
            {
                Orientation.Up => new(searchPos.x, Mathf.Max(chunkPos.y * chunkSize, searchPos.y + 1)),
                Orientation.Right => new(Mathf.Max(chunkPos.x * chunkSize, searchPos.x + 1), searchPos.y),
                Orientation.Down => new(searchPos.x, Mathf.Min((chunkPos.y * chunkSize) + chunkSize - 1, searchPos.y - 1)),
                Orientation.Left => new(Mathf.Min((chunkPos.x * chunkSize) + chunkSize - 1, searchPos.x - 1), searchPos.y),
                _ => Vector2Int.zero,
            };
        }
    }
}
