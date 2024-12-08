using Game.Data;
using System;
using Unity.Plastic.Newtonsoft.Json;
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

            GridData grid = new(data.gridName, data.spacing, data.size);

            foreach (OpticComponent component in data.placedComponents)
                grid.AddComponent(component);

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
                placedComponents = value.placedComponents.ToArray()
            };

            serializer.Serialize(writer, data);
        }
        #endregion
    }
}
