using Game.Data;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Game
{
    public class Vector2JsonConverter : JsonConverter<Vector2>
    {
        #region Read
        public override Vector2 ReadJson(
            JsonReader reader,
            Type objectType,
            Vector2 existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            float[] values = serializer.Deserialize<float[]>(reader);

            return new(values[0], values[1]);
        }
        #endregion

        #region Write
        public override void WriteJson(
            JsonWriter writer,
            Vector2 value,
            JsonSerializer serializer)
        {
            float[] valueArray = new float[2] { value.x, value.y };

            JsonWriteUtils.WriteArrayValues(
                writer,
                serializer,
                valueArray);
        }
        #endregion
    }
}
