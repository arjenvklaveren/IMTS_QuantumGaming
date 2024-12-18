using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    public abstract class OpticComponent
    {
        public static event Action<IEnumerator> OnStartProcessPhotonRoutine;
        public static event Action<Photon> OnPhotonExit;

        public event Action<Orientation> OnOrientationChanged;

        public abstract OpticComponentType Type { get; }

        public GridData HostGrid { get; private set; }

        public HashSet<Vector2Int> OccupiedTiles { get; protected set; }

        [ComponentContext("Root position", "SetRootTile")] 
        public readonly Vector2Int occupiedRootTile;
        [ComponentContext("Orientation", "SetOrientation")] 
        public Orientation orientation;

        public ComponentPort[] InPorts { get; protected set; }
        public ComponentPort[] OutPorts { get; protected set; }

        #region Init
        public OpticComponent(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            ComponentPort[] inPorts,
            ComponentPort[] outPorts)
        {
            HostGrid = hostGrid;

            OccupiedTiles = new(tilesToOccupy);
            occupiedRootTile = GetOccupiedRootTile(tilesToOccupy);

            this.orientation = orientation;

            InPorts = GetPortCopies(inPorts);
            OutPorts = GetPortCopies(outPorts);
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
            InitPortValues(InPorts);
            InitPortValues(OutPorts);
        }

        private void InitPortValues(ComponentPort[] ports)
        {
            int idCounter = 0;

            foreach (ComponentPort port in ports)
            {
                port.owner = this;
                port.portId = idCounter;

                port.position += occupiedRootTile;

                port.OnDetectPhoton += StartHandlePhotonRoutine;

                idCounter++;
            }
        }
        #endregion

        #region Host Grid
        public void AssignHostGrid(GridData hostGrid) => HostGrid = hostGrid;
        #endregion

        #region Rotation
        /// <summary>
        /// This function should only be called throught the GridController class!
        /// </summary>
        /// <param name="increments"></param>
        public void RotateClockwise(int increments = 1)
        {
            Orientation targetOrientation = orientation.RotateClockwise(increments);
            int incrementsToRotate = OrientationUtils.GetRotationDifferenceInClockwiseIncrements(orientation, targetOrientation);

            for (int i = 0; i < incrementsToRotate; i++)
                RotateComponent90Degrees();

            orientation = targetOrientation;
            OnOrientationChanged?.Invoke(orientation);
        }

        private void RotateComponent90Degrees()
        {
            RotateOccupiedTiles90Degrees();
            RotatePorts90Degrees();
        }

        #region Rotate Occupied Tiles
        private void RotateOccupiedTiles90Degrees()
        {
            HashSet<Vector2Int> rotatedOccupiedTiles = new();

            foreach (Vector2Int tile in OccupiedTiles)
                rotatedOccupiedTiles.Add(Get90DegreeRotatedTile(tile));

            OccupiedTiles = rotatedOccupiedTiles;
        }

        private Vector2Int Get90DegreeRotatedTile(Vector2Int tile)
        {
            // Bring position back to origin.
            tile -= occupiedRootTile;

            // Rotate 90 degrees.
            tile = new(tile.y, -tile.x);

            // Bring back to root tile.
            tile += occupiedRootTile;

            return tile;
        }
        #endregion

        #region Rotate Ports
        private void RotatePorts90Degrees()
        {
            foreach (ComponentPort port in InPorts)
                RotatePort90Degrees(port);

            foreach (ComponentPort port in OutPorts)
                RotatePort90Degrees(port);
        }

        private void RotatePort90Degrees(ComponentPort port)
        {
            // Bring port to origin.
            port.position -= occupiedRootTile;

            // Rotate 90 degrees.
            port.position = new(port.position.y, -port.position.x);

            // Bring port back to component.
            port.position += occupiedRootTile;

            // Rotate port orientation.
            port.orientation = port.orientation.RotateClockwise();
        }
        #endregion
        #endregion

        #region Handle Photon
        protected void StartHandlePhotonRoutine(ComponentPort port, Photon photon)
        {
            OnStartProcessPhotonRoutine?.Invoke(HandlePhotonCo(port, photon));
        }

        protected virtual IEnumerator HandlePhotonCo(ComponentPort port, Photon photon) { yield break; }

        protected void TriggerOnPhotonExit(Photon photon)
        {
            OnPhotonExit?.Invoke(photon);
        }
        #endregion

        public virtual void Destroy() { }
        public virtual void Reset() { }

        public virtual void SetOrientation(Orientation orientation) { }
        public void SetRootTile(Vector2Int occupiedRootTile) { }

        public virtual string SerializeArgs() { return ""; }

        #region Generate Local Offset Ports
        public ComponentPort[] GenerateLocalOffsetInPorts() => GenerateLocalOffsetPorts(InPorts);
        public ComponentPort[] GenerateLocalOffsetOutPorts() => GenerateLocalOffsetPorts(OutPorts);

        private ComponentPort[] GenerateLocalOffsetPorts(ComponentPort[] ports)
        {
            ComponentPort[] localOffsetPorts = new ComponentPort[ports.Length];

            for (int i = 0; i < ports.Length; i++)
            {
                ComponentPort port = ports[i];

                localOffsetPorts[i] = new(
                    port.owner,
                    port.position - occupiedRootTile,
                    port.orientation,
                    port.portId);
            }

            return localOffsetPorts;
        }
        #endregion
    }
}
