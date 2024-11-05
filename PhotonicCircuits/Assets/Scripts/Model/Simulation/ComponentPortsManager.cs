using Game.Data;
using SadUtils;
using UnityEngine;

namespace Game
{
    public class ComponentPortsManager : Singleton<ComponentPortsManager>
    {
        [field: SerializeField] public Vector2Int portChunkSize { get; private set; }

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
            Vector2Int chunkPos = GetChunkPosByWorldPos(port.position);

            if (!grid.inPortData.ContainsKey(chunkPos))
                grid.inPortData.Add(chunkPos, new());

            grid.inPortData[chunkPos].RegisterPorts(port);
        }

        private Vector2Int GetChunkPosByWorldPos(Vector2Int worldPos)
        {
            Vector2 worldPosFloat = worldPos;
            int chunkXPos = Mathf.FloorToInt(worldPosFloat.x / portChunkSize.x);
            int chunkYPos = Mathf.FloorToInt(worldPosFloat.y / portChunkSize.y);
            return new(chunkXPos, chunkYPos);
        }
        #endregion
    }
}
