using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ICComponentBase : OpticComponent
    {
        public static event Action OnAnyBlueprintNameChanged;

        public event Action<Photon, ComponentPort> OnPhotonExitIC;
        public event Action<string> OnNameChanged;

        public override OpticComponentType Type => type;

        public Dictionary<string, int> containedBlueprints;

        public GridData InternalGrid { get; private set; }

        public bool IsDirty { get; private set; }

        private List<ICInComponent> inComponents;
        private List<ICOutComponent> outComponents;

        private Vector2Int localBounds;
        private readonly OpticComponentType type;

        #region Constructors
        // Used when creating a new Blueprint
        public ICComponentBase(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            Vector2Int internalGridSize,
            Vector2 internalGridSpacing,
            string title,
            OpticComponentType type) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                orientation,
                new ComponentPort[0],
                new ComponentPort[0])
        {
            InternalGrid = new(title, internalGridSpacing, internalGridSize, true);
            this.type = type;

            SetDefaultValues();
            SetupListeners();
        }

        // Used when placing existing blueprint
        public ICComponentBase(
            GridData hostGrid,
            Vector2Int[] tilesToOccupy,
            Orientation orientation,
            ICBlueprintData data) : base(
                hostGrid,
                tilesToOccupy,
                orientation,
                orientation,
                new ComponentPort[0],
                new ComponentPort[0])
        {
            InternalGrid = new(data.internalGrid);
            type = data.type;

            ComponentPortsManager.Instance.CompileComponentPorts(InternalGrid);

            SetDefaultValues();
            containedBlueprints = new(data.containedBlueprints);

            FindIOComponents();

            SetupListeners();
        }

        private void SetDefaultValues()
        {
            InternalGrid.placementCondition = CanPlaceComponent;

            containedBlueprints = new();
            inComponents = new();
            outComponents = new();

            localBounds = GetOccupiedLocalBounds();
        }

        private void FindIOComponents()
        {
            foreach (OpticComponent component in InternalGrid.placedComponents)
            {
                if (component.Type == OpticComponentType.ICIn)
                    inComponents.Add(component as ICInComponent);
                else if (component.Type == OpticComponentType.ICOut)
                {
                    ICOutComponent icOutComponent = component as ICOutComponent;

                    outComponents.Add(icOutComponent);
                    icOutComponent.OnDetectPhoton += ICOutComponent_OnDetectPhoton;
                }
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
            SetupInternalGridListeners();
            ICBlueprintManager.Instance.OnBlueprintUpdated += ICBlueprintManager_OnBlueprintUpdated;
        }

        private void SetupInternalGridListeners()
        {
            InternalGrid.OnComponentAdded += GridData_OnComponentAdded;
            InternalGrid.OnComponentRemoved += GridData_OnComponentRemoved;
            InternalGrid.OnBlueprintNamed += GridData_OnBlueprintNamed;
            InternalGrid.OnBlueprintRenamed += GridData_OnBlueprintRenamed;
        }
        #endregion

        #region Handle Events
        private void GridData_OnComponentAdded(OpticComponent component) => TryAddPortHandlerComponent(component);
        private void GridData_OnComponentRemoved(OpticComponent component) => TryRemovePortHandlerComponent(component);
        private void GridData_OnBlueprintNamed(string name) => RegisterNamedBlueprint(name);
        private void GridData_OnBlueprintRenamed(string oldName, string newName) => RegisterBlueprintNameChange(oldName, newName);
        private void ICBlueprintManager_OnBlueprintUpdated(ICBlueprintData data) => SyncToBlueprint(data);

        #region Handle Add Component
        private void TryAddPortHandlerComponent(OpticComponent component)
        {
            if (component.Type == OpticComponentType.ICIn)
                AddInComponent(component as ICInComponent);
            else if (component.Type == OpticComponentType.ICOut)
                AddOutComponent(component as ICOutComponent);

            if (TryGetBlueprintComponent(component, out ICComponentBase blueprintComponent))
                HandleAddBlueprintComponent(blueprintComponent);

            IsDirty = true;
        }

        private void AddInComponent(ICInComponent component)
        {
            inComponents.Add(component);

            GenerateInPorts();
        }

        private void AddOutComponent(ICOutComponent component)
        {
            component.portId = outComponents.Count;
            outComponents.Add(component);

            component.OnDetectPhoton += ICOutComponent_OnDetectPhoton;

            GenerateOutPorts();
        }

        private void RegisterNamedBlueprint(string name)
        {
            AddContainedBlueprintInstance(name);
        }

        private void RegisterBlueprintNameChange(string oldName, string newName)
        {
            RemoveBlueprintInstance(oldName);

            AddContainedBlueprintInstance(newName);
        }
        #endregion

        #region Handle Remove Component
        private void TryRemovePortHandlerComponent(OpticComponent component)
        {
            if (component.Type == OpticComponentType.ICIn)
                RemoveInComponent(component as ICInComponent);
            else if (component.Type == OpticComponentType.ICOut)
                RemoveOutComponent(component as ICOutComponent);

            if (TryGetBlueprintComponent(component, out ICComponentBase blueprintComponent))
                HandleRemoveBlueprintComponent(blueprintComponent);

            IsDirty = true;
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
        #endregion

        #region Handle Add / Remove Blueprint Component
        private bool TryGetBlueprintComponent(OpticComponent opticComponent, out ICComponentBase blueprintComponent)
        {
            blueprintComponent = null;

            int typeId = (int)opticComponent.Type;
            if (typeId < 100 || typeId >= 200)
                return false;

            blueprintComponent = opticComponent as ICComponentBase;
            return !string.IsNullOrEmpty(blueprintComponent.InternalGrid.gridName);
        }

        #region Add Blueprint
        private void HandleAddBlueprintComponent(ICComponentBase component)
        {
            string blueprintName = component.InternalGrid.gridName;

            AddContainedBlueprintInstance(blueprintName);

            foreach (string nestedBlueprint in component.containedBlueprints.Keys)
                AddContainedBlueprintInstance(nestedBlueprint);
        }

        private void AddContainedBlueprintInstance(string blueprintName)
        {
            if (!containedBlueprints.ContainsKey(blueprintName))
                containedBlueprints.Add(blueprintName, 0);

            containedBlueprints[blueprintName]++;
        }
        #endregion

        #region Remove Blueprint
        private void HandleRemoveBlueprintComponent(ICComponentBase component)
        {
            string blueprintName = component.InternalGrid.gridName;
            RemoveBlueprintInstance(blueprintName);

            foreach (string blueprint in component.containedBlueprints.Keys)
                RemoveBlueprintInstance(blueprint);
        }

        private void RemoveBlueprintInstance(string blueprint)
        {
            containedBlueprints[blueprint]--;

            if (containedBlueprints[blueprint] <= 0)
                containedBlueprints.Remove(blueprint);
        }
        #endregion
        #endregion

        #region Handle Blueprint Changed
        public void SyncToBlueprint(ICBlueprintData data)
        {
            if (InternalGrid.gridName != data.Name)
                return;

            RemoveOldListeners();

            containedBlueprints = new(data.containedBlueprints);
            InternalGrid = new(data.internalGrid)
            {
                placementCondition = CanPlaceComponent
            };

            SetupInternalGridListeners();

            ResetIOComponentData();
            FindIOComponents();

            // Ensure that serialization system doesn't try to save synced blueprints
            IsDirty = false;
        }

        private void RemoveOldListeners()
        {
            InternalGrid.OnComponentAdded -= GridData_OnComponentAdded;
            InternalGrid.OnComponentRemoved -= GridData_OnComponentRemoved;
            InternalGrid.OnBlueprintNamed -= GridData_OnBlueprintNamed;
            InternalGrid.OnBlueprintRenamed -= GridData_OnBlueprintRenamed;
        }

        private void ResetIOComponentData()
        {
            inComponents.Clear();
            outComponents.Clear();
        }
        #endregion
        #endregion

        #region Handle Place Condition
        private bool CanPlaceComponent(OpticComponent component)
        {
            if (component.Type == OpticComponentType.ICIn)
                return CanAddIOComponentInOrientation(component.orientation);

            else if (component.Type == OpticComponentType.ICOut)
                return CanAddIOComponentInOrientation(component.orientation);

            else if (TryGetBlueprintComponent(component, out ICComponentBase blueprintComponent))
                return CanPlaceBlueprintComponent(blueprintComponent);

            return true;
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

        private bool CanPlaceBlueprintComponent(ICComponentBase blueprintComponent)
        {
            string blueprintName = blueprintComponent.InternalGrid.gridName;

            if (string.IsNullOrEmpty(blueprintName))
                return true;

            if (blueprintName == InternalGrid.gridName)
                return false;

            if (!ICBlueprintManager.Instance.TryGetBlueprintData(blueprintName, out ICBlueprintData blueprintData))
                return true;

            bool blueprintContainsSelf = blueprintData.containedBlueprints.ContainsKey(InternalGrid.gridName);
            return !blueprintContainsSelf;
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

        #region Handle Name
        public void SetName(string name)
        {
            InternalGrid.gridName = name;

            OnAnyBlueprintNameChanged?.Invoke();
            OnNameChanged?.Invoke(name);
        }
        #endregion

        #region Handle Photon
        protected override IEnumerator HandlePhotonCo(ComponentPort port, Photon photon)
        {
            photon.currentGrid = InternalGrid;
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

        #region Rotation
        public override void SetOrientation(Orientation orientation) => ComponentRotateUtil.SetOrientation(this, orientation);
        #endregion

        #region Serialization
        public override string SerializeArgs()
        {
            IsDirty = false;

            return InternalGrid.gridName;
        }
        #endregion
    }
}
