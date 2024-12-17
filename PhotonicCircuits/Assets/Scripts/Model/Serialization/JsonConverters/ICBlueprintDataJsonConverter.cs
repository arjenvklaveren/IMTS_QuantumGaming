using Game.Data;
using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;

namespace Game
{
    public class ICBlueprintDataJsonConverter : JsonConverter<ICBlueprintData>
    {
        private struct BlueprintData
        {
            public Dictionary<string, int> containedBlueprints;

            public GridData internalGrid;
            public OpticComponentType type;
        }

        #region Read
        public override ICBlueprintData ReadJson(
            JsonReader reader,
            Type objectType,
            ICBlueprintData existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            BlueprintData blueprintData = serializer.Deserialize<BlueprintData>(reader);

            return new(
                blueprintData.containedBlueprints,
                blueprintData.internalGrid,
                blueprintData.type);
        }
        #endregion

        #region Write
        public override void WriteJson(
            JsonWriter writer,
            ICBlueprintData value,
            JsonSerializer serializer)
        {
            BlueprintData blueprintData = new()
            {
                containedBlueprints = value.containedBlueprints,

                internalGrid = value.internalGrid,
                type = value.type
            };

            serializer.Serialize(writer, blueprintData);
        }
        #endregion
    }
}
