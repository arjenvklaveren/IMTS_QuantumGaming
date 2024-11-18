using Game.Data;
using System;
using Unity.Plastic.Newtonsoft.Json;

namespace Game
{
    public class ComponentPortJsonConverter : JsonConverter<ComponentPort>
    {
        #region Read
        public override ComponentPort ReadJson(JsonReader reader, Type objectType, ComponentPort existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Write
        public override void WriteJson(
            JsonWriter writer,
            ComponentPort value,
            JsonSerializer serializer)
        {
            writer.WriteStartObject();

            JsonWriteUtils.WriteProperty(
                writer,
                serializer,
                nameof(value.position),
                value.position);

            JsonWriteUtils.WriteProperty(
                writer,
                serializer,
                nameof(value.orientation),
                value.orientation);

            writer.WriteEndObject();
        }
        #endregion
    }
}
