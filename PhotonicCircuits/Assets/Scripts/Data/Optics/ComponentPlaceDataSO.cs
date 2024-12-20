using UnityEngine;

namespace Game.Data
{
    public abstract class ComponentPlaceDataSO : ScriptableObject
    {
        public string title;

        [Header("Preview Settings")]
        public Sprite previewSprite;
        public Vector2 previewScale;
        public Vector2Int previewTileOffset;

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
        public Vector2Int[] GetTilesToOccupy(Vector2Int position)
        {
            if (tileOffsetsToOccupy == null || tileOffsetsToOccupy.Length == 0)
                return new Vector2Int[1] { position };

            Vector2Int[] tilesToOccupy = new Vector2Int[tileOffsetsToOccupy.Length];

            for (int i = 0; i < tileOffsetsToOccupy.Length; i++)
                tilesToOccupy[i] = position + tileOffsetsToOccupy[i];

            return tilesToOccupy;
        }

        public Vector2Int[] GetRotatedTilesToOccupy(Vector2Int[] tiles, Orientation placeOrientation)
        {
            Vector2Int[] rotatedTiles = new Vector2Int[tiles.Length];

            Vector2Int position = tiles[0] - tileOffsetsToOccupy[0];
            int incrementsToRotate = defaultOrientation.GetClockwiseIncrementsDiff(placeOrientation);

            for (int i = 0; i < tiles.Length; i++)
                rotatedTiles[i] = GetRotatedTile(tiles[i], position, incrementsToRotate);

            return rotatedTiles;
        }

        private Vector2Int GetRotatedTile(Vector2Int tile, Vector2Int position, int increments)
        {
            tile -= position;

            for (int i = 0; i < increments; i++)
                tile = new(tile.y, -tile.x);

            return tile + position;
        }
        #endregion
    }
}
