using Game.Data;
using Newtonsoft.Json;
using System;
using UnityEngine;

namespace Game
{
    public class Vector2IntJsonConverter : JsonConverter<Vector2Int>
    {
        #region Read
        public override Vector2Int ReadJson(
            JsonReader reader,
            Type objectType,
            Vector2Int existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            int[] values = serializer.Deserialize<int[]>(reader);

            return new(values[0], values[1]);
        }
        #endregion

        #region Write
        public override void WriteJson(
            JsonWriter writer,
            Vector2Int value,
            JsonSerializer serializer)
        {
            int[] valueArray = new int[2] { value.x, value.y };

            JsonWriteUtils.WriteArrayValues(
                writer,
                serializer,
                valueArray);
        }
        #endregion
    }
}
