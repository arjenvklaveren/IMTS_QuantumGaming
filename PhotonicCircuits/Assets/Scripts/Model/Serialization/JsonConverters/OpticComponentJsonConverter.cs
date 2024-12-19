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

            public Orientation orientation;

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
                OpticComponentType.SourceSingle => LoadSingleSourceComponent(data),
                OpticComponentType.SourceLaser => LoadLaserSourceComponent(data),
                OpticComponentType.Mirror => LoadMirrorComponent(data),
                OpticComponentType.BeamSplitter => LoadBeamSplitterComponent(data),
                OpticComponentType.PhaseShifter => LoadPhaseShifterComponent(data),
                OpticComponentType.Detector => LoadDetectorComponent(data),

                // IC Formats
                OpticComponentType.IC1x1 => LoadIC1x1Component(data),
                OpticComponentType.IC2x2 => LoadIC2x2Component(data),

                // IC
                OpticComponentType.ICIn => LoadICInComponent(data),
                OpticComponentType.ICOut => LoadICOutComponent(data),

                _ => throw new NotImplementedException($"Deserialization for component type {data.type} has not been implemented!"),
            };
        }

        #region Load Component Types
        private TestComponent LoadTestComponent(OpticComponentData data)
        {
            return new(
                null,
                data.occupiedTiles,
                data.orientation,
                data.orientation,
                data.inPorts,
                data.outPorts);
        }

        private PhotonSingleSourceComponent LoadSingleSourceComponent(OpticComponentData data)
        {
            return new(
                null,
                data.occupiedTiles,
                data.orientation,
                data.orientation,
                data.inPorts,
                data.outPorts);
        }

        private PhotonLaserSourceComponent LoadLaserSourceComponent(OpticComponentData data)
        {
            return new(
                null,
                data.occupiedTiles,
                data.orientation,
                data.orientation,
                data.inPorts,
                data.outPorts);
        }

        private MirrorComponent LoadMirrorComponent(OpticComponentData data)
        {
            return new(
                null,
                data.occupiedTiles,
                data.orientation,
                data.orientation,
                data.inPorts,
                data.outPorts);
        }

        private BeamSplitterComponent LoadBeamSplitterComponent(OpticComponentData data)
        {
            return new(
                null,
                data.occupiedTiles,
                data.orientation,
                data.orientation,
                data.inPorts,
                data.outPorts);
        }

        private PhaseShifterComponent LoadPhaseShifterComponent(OpticComponentData data)
        {
            float shiftParse = 0;
            float.TryParse(data.args, out shiftParse);

            return new(
                null,
                data.occupiedTiles,
                data.orientation,
                data.orientation,
                data.inPorts,
                data.outPorts,
                shiftParse);
        }

        private PhotonDetectorComponent LoadDetectorComponent(OpticComponentData data)
        {
            return new(
                null,
                data.occupiedTiles,
                data.orientation,
                data.orientation,
                data.inPorts,
                data.outPorts);
        }

        #region IC
        #region IC Formats
        private ICComponent1x1 LoadIC1x1Component(OpticComponentData data)
        {
            if (!ICBlueprintManager.TryGetBlueprintData(data.args, out ICBlueprintData blueprintData))
                throw new Exception("Invalid Save data encountered!");

            return new(
                null,
                data.occupiedTiles,
                data.orientation,
                blueprintData);
        }

        private ICComponent2x2 LoadIC2x2Component(OpticComponentData data)
        {
            if (!ICBlueprintManager.TryGetBlueprintData(data.args, out ICBlueprintData blueprintData))
                throw new Exception("Invalid Save data encountered!");

            return new(
                null,
                data.occupiedTiles,
                data.orientation,
                blueprintData);
        }
        #endregion

        private ICInComponent LoadICInComponent(OpticComponentData data)
        {
            return new(
                null,
                data.occupiedTiles,
                data.orientation,
                data.orientation,
                data.inPorts,
                data.outPorts);
        }

        private ICOutComponent LoadICOutComponent(OpticComponentData data)
        {
            int portId = JsonConvert.DeserializeObject<int>(data.args);

            return new(
                null,
                data.occupiedTiles,
                data.orientation,
                data.orientation,
                data.inPorts,
                data.outPorts,
                portId);
        }
        #endregion
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
                occupiedTiles = OccupiedTilesToArray(value.OccupiedTiles),
                orientation = value.orientation,
                inPorts = value.InPorts,
                outPorts = value.OutPorts,
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

        private ICBlueprintManager ICBlueprintManager => ICBlueprintManager.Instance;
    }
}
