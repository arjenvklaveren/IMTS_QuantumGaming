using Game.Data;
using SadUtils;
using UnityEngine;

namespace Game
{
    public class PhotonPathManager : Singleton<PhotonPathManager>
    {
        protected override void Awake()
        {
            SetInstance(this);
        }

        #region Find In Port
        public bool TryFindNextInPort(
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

        private bool TrySearchChunk(
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
