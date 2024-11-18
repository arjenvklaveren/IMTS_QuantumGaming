using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

namespace Game.Data
{
    public static class JsonWriteUtils
    {
        public static void WriteProperty<T>(
            JsonWriter writer,
            JsonSerializer serializer,
            string propertyName,
            T value)
        {
            writer.WritePropertyName(propertyName);
            serializer.Serialize(writer, value);
        }

        public static void WriteArrayProperty<T>(
            JsonWriter writer,
            JsonSerializer serializer,
            string propertyName,
            IEnumerable<T> values)
        {
            writer.WritePropertyName(propertyName);

            WriteArrayValues(
                writer,
                serializer,
                values);
        }

        public static void WriteArrayValues<T>(
            JsonWriter writer,
            JsonSerializer serializer,
            IEnumerable<T> values)
        {
            writer.WriteStartArray();

            foreach (T value in values)
                serializer.Serialize(writer, value);

            writer.WriteEndArray();
        }
    }
}
