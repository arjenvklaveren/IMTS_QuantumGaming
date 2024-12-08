using Game.Data;
using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class OpticComponentJsonConverter : JsonConverter<OpticComponent>
    {
        public struct OpticComponentData
        {
            public OpticComponentType type;
            public Vector2Int[] occupiedTiles;

            public ComponentPort[] inPorts;
            public ComponentPort[] outPorts;

            public string args;
        }

        #region Read
        public override OpticComponent ReadJson(
            JsonReader reader,
            Type objectType,
            OpticComponent existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            OpticComponentData data = serializer.Deserialize<OpticComponentData>(reader);

            return data.type switch
            {
                OpticComponentType.Test => LoadTestComponent(data),
                OpticComponentType.Source => LoadSourceComponent(data),
                _ => throw new NotImplementedException($"Deserialization for component type {data.type} has not been implemented!"),
            };
        }

        #region Load Component Types
        private TestComponent LoadTestComponent(OpticComponentData data)
        {
            return new TestComponent(data.occupiedTiles, data.inPorts, data.outPorts);
        }

        private PhotonSourceComponent LoadSourceComponent(OpticComponentData data)
        {
            return new PhotonSourceComponent(data.occupiedTiles, data.inPorts, data.outPorts);
        }
        #endregion
        #endregion

        #region Write
        // Serialization of inherited class args is implemented in OpticComponent class.
        public override void WriteJson(
            JsonWriter writer,
            OpticComponent value,
            JsonSerializer serializer)
        {
            OpticComponentData data = new()
            {
                type = value.Type,
                occupiedTiles = OccupiedTilesToArray(value.occupiedTiles),
                inPorts = value.inPorts,
                outPorts = value.outPorts,
                args = value.SerializeArgs()
            };

            serializer.Serialize(writer, data);
        }

        private Vector2Int[] OccupiedTilesToArray(HashSet<Vector2Int> input)
        {
            Vector2Int[] occupiedTiles = new Vector2Int[input.Count];
            int counter = 0;

            foreach (Vector2Int tile in input)
            {
                occupiedTiles[counter] = tile;
                counter++;
            }

            return occupiedTiles;
        }
        #endregion
    }
}
