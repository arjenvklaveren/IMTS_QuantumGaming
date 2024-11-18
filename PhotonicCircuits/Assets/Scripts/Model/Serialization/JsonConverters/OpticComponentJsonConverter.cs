using Game.Data;
using System;
using Unity.Plastic.Newtonsoft.Json;

namespace Game
{
    public class OpticComponentJsonConverter : JsonConverter<OpticComponent>
    {
        #region Read
        public override OpticComponent ReadJson(JsonReader reader, Type objectType, OpticComponent existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Write
        // Serialization is implemented in optic component class.
        public override void WriteJson(
            JsonWriter writer,
            OpticComponent value,
            JsonSerializer serializer)
        {
            value.SerializeToJson(writer, serializer);
        }
        #endregion
    }
}
