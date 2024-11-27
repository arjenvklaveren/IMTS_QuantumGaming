using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public abstract class ICComponentBase : OpticComponent
    {
        public event Action<Photon, ComponentPort> OnPhotonExitIC;

        public readonly GridData internalGrid;

        protected List<ICInComponent> inComponents;
        protected List<ICOutComponent> outComponents;

        private Vector2Int localBounds;

        protected bool isDirty;

        #region Constructor
        protected ICComponentBase(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            Vector2Int internalGridSize,
            Vector2 internalGridSpacing,
            string title) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                new ComponentPort[0],
                new ComponentPort[0])
        {
            internalGrid = new(title, internalGridSpacing, internalGridSize, true);
            SetDefaultValues();
            SetupListeners();
        }

        protected ICComponentBase(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            GridData internalGrid) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                new ComponentPort[0],
                new ComponentPort[0])
        {
            this.internalGrid = internalGrid;
            SetDefaultValues();
            FindIOComponents();

            SetupListeners();
        }

        private void SetDefaultValues()
        {
            inComponents = new();
            outComponents = new();

            localBounds = GetOccupiedLocalBounds();
        }

        private void FindIOComponents()
        {
            foreach (OpticComponent component in internalGrid.placedComponents)
            {
                if (component.Type == OpticComponentType.ICIn)
                    inComponents.Add(component as ICInComponent);
                else if (component.Type == OpticComponentType.ICOut)
                    outComponents.Add(component as ICOutComponent);
            }

            GenerateInPorts();
            GenerateOutPorts();
        }

        private Vector2Int GetOccupiedLocalBounds()
        {
            Vector2Int min = occupiedRootTile, max = occupiedRootTile;

            foreach (Vector2Int tile in OccupiedTiles)
            {
                if (tile.x < min.x)
                    min.x = tile.x;
                else if (tile.x > max.x)
                    max.x = tile.x;

                if (tile.y < min.y)
                    min.y = tile.y;
                else if (tile.y > max.y)
                    max.y = tile.y;
            }

            return max - min;
        }

        private void SetupListeners()
        {
            internalGrid.OnComponentAdded += GridData_OnComponentAdded;
            internalGrid.OnComponentRemoved += GridData_OnComponentRemoved;
        }
        #endregion

        #region Handle Events
        private void GridData_OnComponentAdded(OpticComponent component) => TryAddPortHandlerComponent(component);
        private void GridData_OnComponentRemoved(OpticComponent component) => TryRemovePortHandlerComponent(component);

        private void TryAddPortHandlerComponent(OpticComponent component)
        {
            if (component.Type == OpticComponentType.ICIn)
                AddInComponent(component as ICInComponent);
            else if (component.Type == OpticComponentType.ICOut)
                AddOutComponent(component as ICOutComponent);

            isDirty = true;
        }

        private void AddInComponent(ICInComponent component)
        {

            if (CanAddIOComponentInOrientation(component.orientation))
            {
                inComponents.Add(component);

                GenerateInPorts();
            }
            else
                ExternalCoroutineExecutionManager.Instance.StartSimulationCoroutine(RemoveComponentCo(component));
        }

        private void AddOutComponent(ICOutComponent component)
        {
            if (CanAddIOComponentInOrientation(component.orientation))
            {
                component.portId = outComponents.Count;
                outComponents.Add(component);

                component.OnDetectPhoton += ICOutComponent_OnDetectPhoton;

                GenerateOutPorts();
            }
            else
                ExternalCoroutineExecutionManager.Instance.StartSimulationCoroutine(RemoveComponentCo(component));
        }

        private void TryRemovePortHandlerComponent(OpticComponent component)
        {
            if (component.Type == OpticComponentType.ICIn)
                RemoveInComponent(component as ICInComponent);
            else if (component.Type == OpticComponentType.ICOut)
                RemoveOutComponent(component as ICOutComponent);

            isDirty = true;
        }

        private void RemoveInComponent(ICInComponent component)
        {
            inComponents.Remove(component);

            GenerateInPorts();
        }

        private void RemoveOutComponent(ICOutComponent component)
        {
            outComponents.Remove(component);

            component.OnDetectPhoton -= ICOutComponent_OnDetectPhoton;

            // Recalculate out component port Ids
            for (int i = 0; i < outComponents.Count; i++)
                outComponents[i].portId = i;

            GenerateOutPorts();
        }

        private bool CanAddIOComponentInOrientation(Orientation componentOrientation)
        {
            Orientation portOrientation = componentOrientation.RotateClockwise(2);

            int counter = 0;

            foreach (ComponentPort port in InPorts)
                if (port.orientation == portOrientation)
                    counter++;

            foreach (ComponentPort port in OutPorts)
                if (port.orientation == portOrientation)
                    counter++;

            return portOrientation switch
            {
                Orientation.Up or Orientation.Down => counter < localBounds.x + 1,
                Orientation.Right or Orientation.Left => counter < localBounds.y + 1,
                _ => false,
            };
        }

        private IEnumerator RemoveComponentCo(OpticComponent component)
        {
            yield return null;
            GridManager.Instance.GridController.TryRemoveComponent(component);
        }
        #endregion

        #region Generate Ports
        private void GenerateInPorts()
        {
            InPorts = new ComponentPort[inComponents.Count];

            for (int i = 0; i < inComponents.Count; i++)
                InPorts[i] = GeneratePort(inComponents[i], i);
        }

        private void GenerateOutPorts()
        {
            OutPorts = new ComponentPort[outComponents.Count];

            for (int i = 0; i < outComponents.Count; i++)
                OutPorts[i] = GeneratePort(outComponents[i], outComponents[i].portId);
        }

        private ComponentPort GeneratePort(OpticComponent ioComponent, int portId)
        {
            Orientation portOrientation = ioComponent.orientation.RotateClockwise(2);

            ComponentPort port = new(
                this,
                GetPortPosition(portOrientation),
                portOrientation,
                portId);

            port.OnDetectPhoton += StartHandlePhotonRoutine;

            return port;
        }

        private Vector2Int GetPortPosition(Orientation portOrientation)
        {
            Vector2Int pos = occupiedRootTile + GetRelativeStartPosition(portOrientation);
            int counter = 0;

            List<ComponentPort> ports = new(InPorts);
            ports.AddRange(OutPorts);

            foreach (ComponentPort port in ports)
                if (port != null)
                    if (port.orientation == portOrientation)
                        counter++;

            if (counter > 0)
                pos += counter * GetStackDir(portOrientation);

            return pos;
        }

        private Vector2Int GetRelativeStartPosition(Orientation orientation)
        {
            return orientation switch
            {
                Orientation.Down => new(0, -localBounds.y),
                Orientation.Right => new(localBounds.x, 0),
                _ => Vector2Int.zero
            };
        }

        private Vector2Int GetStackDir(Orientation orientation)
        {
            return orientation switch
            {
                Orientation.Up => Vector2Int.right,
                Orientation.Right => Vector2Int.down,
                Orientation.Down => Vector2Int.right,
                Orientation.Left => Vector2Int.down,
                _ => Vector2Int.zero
            };
        }
        #endregion

        #region Handle Photon
        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            photon.currentGrid = internalGrid;
            inComponents[port.portId].HandlePhoton(photon);

            yield break;
        }

        private void ICOutComponent_OnDetectPhoton(Photon photon, int portId)
        {
            ComponentPort port = OutPorts[portId];

            photon.currentGrid = HostGrid;

            photon.SetPosition(port.position);
            photon.SetPropagation(port.orientation);

            photon.TriggerExitComponent(this);
            TriggerOnPhotonExit(photon);

            OnPhotonExitIC?.Invoke(photon, port);
        }
        #endregion

        #region Serialization
        public override string SerializeArgs()
        {
            isDirty = false;

            return JsonConvert.SerializeObject(internalGrid, SerializationManager.GetAllConverters());
        }
        #endregion
    }
}
