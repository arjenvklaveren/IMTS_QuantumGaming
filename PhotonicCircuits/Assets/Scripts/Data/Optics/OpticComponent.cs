using System;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Game.Data
{
    public abstract class OpticComponent
    {
        public static event Action<Photon> OnPhotonExit;

        public abstract OpticComponentType Type { get; }

        public readonly HashSet<Vector2Int> occupiedTiles;
        public readonly Vector2Int occupiedRootTile;

        public readonly ComponentPort[] inPorts;
        public readonly ComponentPort[] outPorts;

        #region Init
        public OpticComponent(
            Vector2Int[] tilesToOccupy,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts)
        {
            occupiedTiles = new(tilesToOccupy);
            occupiedRootTile = GetOccupiedRootTile(tilesToOccupy);

            this.inPorts = GetPortCopies(inPorts);
            this.outPorts = GetPortCopies(outPorts);
            InitPorts();
        }

        protected virtual Vector2Int GetOccupiedRootTile(Vector2Int[] tilesToOccupy)
        {
            return tilesToOccupy[0];
        }

        private ComponentPort[] GetPortCopies(ComponentPort[] ports)
        {
            ComponentPort[] copies = new ComponentPort[ports.Length];

            for (int i = 0; i < ports.Length; i++)
                copies[i] = new(ports[i]);

            return copies;
        }

        private void InitPorts()
        {
            InitPortValues(inPorts);
            InitPortValues(outPorts);
        }

        private void InitPortValues(ComponentPort[] ports)
        {
            int idCounter = 0;

            foreach (ComponentPort port in ports)
            {
                port.owner = this;
                port.portId = idCounter;

                port.position += occupiedRootTile;

                port.OnDetectPhoton += HandlePhoton;

                idCounter++;
            }
        }
        #endregion

        #region Handle Photon
        protected virtual void HandlePhoton(ComponentPort port, Photon photon) { }

        protected void TriggerOnPhotonExit(Photon photon)
        {
            OnPhotonExit?.Invoke(photon);
        }
        #endregion

        public virtual void Destroy() { }

        #region Serialization
        #region Read
        #endregion

        #region Write
        protected JsonWriter writer;
        protected JsonSerializer writeSerializer;

        public void SerializeToJson(
            JsonWriter writer,
            JsonSerializer serializer)
        {
            this.writer = writer;
            writeSerializer = serializer;

            writer.WriteStartObject();

            WriteProperty(nameof(Type), Type);
            WriteProperty(nameof(occupiedTiles), occupiedTiles);

            WriteProperty(nameof(inPorts), inPorts);
            WriteProperty(nameof(outPorts), outPorts);

            // inherit seralization logic
            WriteToJson();

            writer.WriteEndObject();

            DisposeWriteVars();
        }

        private void DisposeWriteVars()
        {
            writer = null;
            writeSerializer = null;
        }

        private void WriteProperty<T>(string name, T value)
        {
            JsonWriteUtils.WriteProperty(
                writer,
                writeSerializer,
                name,
                value);
        }

        protected virtual void WriteToJson() { }
        #endregion
        #endregion
    }
}
