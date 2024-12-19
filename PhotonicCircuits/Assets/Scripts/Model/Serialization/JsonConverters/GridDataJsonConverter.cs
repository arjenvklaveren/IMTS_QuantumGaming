using Game.Data;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Game
{
    public class GridDataJsonConverter : JsonConverter<GridData>
    {
        private struct GridSerializationData
        {
            public string gridName;
            public Vector2 spacing;
            public Vector2Int size;

            public OpticComponent[] placedComponents;

            public bool isIntegrated;
        }

        #region Read
        public override GridData ReadJson(
            JsonReader reader,
            Type objectType,
            GridData existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            GridSerializationData data = serializer.Deserialize<GridSerializationData>(reader);

            GridData grid = new(data.gridName, data.spacing, data.size, data.isIntegrated);

            // Register grid components.
            foreach (OpticComponent component in data.placedComponents)
            {
                grid.AddComponent(component);
                component.AssignHostGrid(grid);
            }

            return grid;
        }
        #endregion

        #region Write
        public override void WriteJson(
            JsonWriter writer,
            GridData value,
            JsonSerializer serializer)
        {
            GridSerializationData data = new()
            {
                gridName = value.gridName,
                spacing = value.spacing,
                size = value.size,
                placedComponents = value.placedComponents.ToArray(),
                isIntegrated = value.isIntegrated,
            };

            serializer.Serialize(writer, data);
        }
        #endregion
    }
}
