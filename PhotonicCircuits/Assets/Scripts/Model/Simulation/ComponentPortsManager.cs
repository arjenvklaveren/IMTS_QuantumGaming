using Game.Data;
using SadUtils;
using UnityEngine;

namespace Game
{
    public class ComponentPortsManager : Singleton<ComponentPortsManager>
    {
        #region Awake / Destroy
        protected override void Awake()
        {
            SetupEventListeners();
            SetInstance(this);
        }

        private void OnDestroy()
        {
            RemoveEventListeners();
        }

        private void SetupEventListeners()
        {
            SimulationManager.OnSimulationInitialize += SimulationManager_OnSimulationInitialize;
        }

        private void RemoveEventListeners()
        {
            SimulationManager.OnSimulationInitialize -= SimulationManager_OnSimulationInitialize;
        }
        #endregion

        #region Handle Events
        private void SimulationManager_OnSimulationInitialize()
        {
            GridData openGrid = GridManager.Instance.GetActiveGrid();
            // Reset port data.
            openGrid.inPortsData.Clear();

            CompileComponentPorts(openGrid);
        }
        #endregion

        #region Compile Component Ports
        public void CompileComponentPorts(GridData grid)
        {
            foreach (OpticComponent component in grid.placedComponents)
                CompilePortsOnComponent(component, grid);
        }

        private void CompilePortsOnComponent(OpticComponent component, GridData grid)
        {
            foreach (ComponentPort port in component.inPorts)
                RegisterPort(port, grid);
        }

        private void RegisterPort(ComponentPort port, GridData grid)
        {
            Vector2Int chunkPos = GridUtils.GridPos2ChunkPos(port.position);

            if (!grid.inPortsData.ContainsKey(chunkPos))
                grid.inPortsData.Add(chunkPos, new());

            grid.inPortsData[chunkPos].RegisterPorts(port);
        }
        #endregion
    }
}
