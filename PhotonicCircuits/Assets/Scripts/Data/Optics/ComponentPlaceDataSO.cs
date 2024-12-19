using UnityEngine;

namespace Game.Data
{
    public abstract class ComponentPlaceDataSO : ScriptableObject
    {
        public string title;

        [Header("Preview Settings")]
        public Sprite previewSprite;
        public Vector2 previewScale;

        [Header("Other Settings")]
        public Sprite iconSprite;

        [Header("Grid Settings")]
        public PlaceRestrictionType restrictionType = PlaceRestrictionType.Both;
        public Vector2Int[] tileOffsetsToOccupy;

        [Header("Orientation")]
        public Orientation defaultOrientation;

        // Port positions are relative!
        [Header("Port Settings")]
        public ComponentPort[] inPorts;
        public ComponentPort[] outPorts;

        public abstract OpticComponent CreateOpticComponent(GridData hostGrid, Vector2Int[] tilesToOccupy, Orientation placeOrientation);

        public Orientation GetPlaceOrientation(Orientation orientationOffset)
        {
            int offsetIncrements = (int)orientationOffset;
            return defaultOrientation.RotateClockwise(offsetIncrements);
        }

        #region Get Tiles To Occupy
        public Vector2Int[] GetTilesToOccupy(Vector2Int position, Orientation placeOrientation)
        {
            if (tileOffsetsToOccupy == null || tileOffsetsToOccupy.Length == 0)
                return new Vector2Int[] { position };

            // Return position with offsets.
            Vector2Int[] tilesToOccupy = new Vector2Int[tileOffsetsToOccupy.Length];

            int rotateIncrements = defaultOrientation.GetClockwiseIncrementsDiff(placeOrientation);

            for (int i = 0; i < tileOffsetsToOccupy.Length; i++)
            {
                Vector2Int rotatedTileOffset = GetRotatedTileOffsetToOccupy(tileOffsetsToOccupy[i], rotateIncrements);
                tilesToOccupy[i] = position + rotatedTileOffset;
            }

            return tilesToOccupy;
        }

        private Vector2Int GetRotatedTileOffsetToOccupy(Vector2Int tile, int rotateIncrements)
        {
            for (int i = 0; i < rotateIncrements; i++)
                RotateTileOffsetClockwise(ref tile);

            return tile;
        }

        private void RotateTileOffsetClockwise(ref Vector2Int tile)
        {
            tile = new(tile.y, -tile.x);
        }
        #endregion
    }
}
