using Game.Data;
using System;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class ComponentPortJsonConverter : JsonConverter<ComponentPort>
    {
        private struct PortData
        {
            public Vector2Int position;
            public Orientation orientation;
        }

        #region Read
        public override ComponentPort ReadJson(
            JsonReader reader,
            Type objectType,
            ComponentPort existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            PortData portData = serializer.Deserialize<PortData>(reader);

            return new ComponentPort(portData.position, portData.orientation);
        }
        #endregion

        #region Write
        public override void WriteJson(
            JsonWriter writer,
            ComponentPort value,
            JsonSerializer serializer)
        {
            PortData portData = new()
            {
                position = value.position - value.owner.occupiedRootTile,
                orientation = value.orientation,
            };

            serializer.Serialize(writer, portData);
        }
        #endregion
    }
}
