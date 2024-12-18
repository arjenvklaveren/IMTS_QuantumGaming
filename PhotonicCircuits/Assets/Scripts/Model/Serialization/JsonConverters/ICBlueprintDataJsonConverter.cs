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
            public List<KeyValuePair<string, int>> containedBlueprintsList;

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
                ListToDictionary(blueprintData.containedBlueprintsList),
                blueprintData.internalGrid,
                blueprintData.type);
        }

        private Dictionary<string, int> ListToDictionary(List<KeyValuePair<string, int>> list)
        {
            Dictionary<string, int> dict = new();

            foreach (KeyValuePair<string, int> pair in list)
                dict.Add(pair.Key, pair.Value);

            return dict;
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
                containedBlueprintsList = DictionaryToList(value.containedBlueprints),

                internalGrid = value.internalGrid,
                type = value.type
            };

            serializer.Serialize(writer, blueprintData);
        }

        private List<KeyValuePair<string, int>> DictionaryToList(Dictionary<string, int> dict)
        {
            List<KeyValuePair<string, int>> list = new();

            foreach (KeyValuePair<string, int> pair in dict)
                list.Add(pair);

            return list;
        }
        #endregion
    }
}
