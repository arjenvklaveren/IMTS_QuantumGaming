using UnityEngine;

namespace Game.Data
{
    public abstract class ComponentPlaceDataSO : ScriptableObject
    {
        public string title;

        [Header("Preview Settings")]
        public Sprite previewSprite;
        public Vector2 previewScale;

        [Header("Grid Settings")]
        public Vector2Int[] tileOffsetsToOccupy;

        // Port positions are relative!
        [Header("Port Settings")]
        public ComponentPort[] inPorts;
        public ComponentPort[] outPorts;

        public abstract OpticComponent CreateOpticComponent(Vector2Int[] tilesToOccupy);

        public Vector2Int[] GetTilesToOccupy(Vector2Int position)
        {
            if (tileOffsetsToOccupy == null || tileOffsetsToOccupy.Length == 0)
                return new Vector2Int[] { position };

            // Return position with offsets.
            Vector2Int[] tilesToOccupy = new Vector2Int[tileOffsetsToOccupy.Length];

            for (int i = 0; i < tileOffsetsToOccupy.Length; i++)
                tilesToOccupy[i] = position + tileOffsetsToOccupy[i];

            return tilesToOccupy;
        }
    }
}
