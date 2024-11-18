using Game.Data;
using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

namespace Game
{
    public class GridDataJsonConverter : JsonConverter<GridData>
    {
        #region Read
        public override GridData ReadJson(
            JsonReader reader,
            Type objectType,
            GridData existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Write
        private JsonWriter writer;
        private JsonSerializer serializer;

        public override void WriteJson(
            JsonWriter writer,
            GridData value,
            JsonSerializer serializer)
        {
            this.writer = writer;
            this.serializer = serializer;

            WriteProperty(nameof(value.gridName), value.gridName);
            WriteProperty(nameof(value.spacing), value.spacing);
            WriteProperty(nameof(value.size), value.size);

            WriteArrayProperty(
                nameof(value.placedComponents),
                value.placedComponents);

            Dispose();
        }

        private void WriteProperty<T>(string name, T value)
        {
            JsonWriteUtils.WriteProperty(
                writer,
                serializer,
                name,
                value);
        }

        private void WriteArrayProperty<T>(
            string name,
            IEnumerable<T> values)
        {
            JsonWriteUtils.WriteArrayProperty(
                writer,
                serializer,
                name,
                values);
        }

        private void Dispose()
        {
            writer = null;
            serializer = null;
        }
        #endregion
    }
}
