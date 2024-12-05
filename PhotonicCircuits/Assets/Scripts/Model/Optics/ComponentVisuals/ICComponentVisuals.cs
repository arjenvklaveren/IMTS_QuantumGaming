using Game.Data;
using UnityEngine;

namespace Game
{
    public class ICComponentVisuals : ComponentVisuals
    {
        [SerializeField] private Transform visualsHolder;

        [Space]
        [SerializeField] private Transform inPortsHolder;
        [SerializeField] private Transform outPortsHolder;

        [Header("Prefab References")]
        [SerializeField] private PhotonVisuals photonPrefab;
        [SerializeField] private GameObject inPortPrefab;
        [SerializeField] private GameObject outPortPrefab;

        private ICComponentBase sourceICComponent;

        #region Create / Destroy
        public override void SetSource(OpticComponent component)
        {
            base.SetSource(component);
            SetDefaultValues();
            GeneratePortVisuals();

            SetupListeners();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveListeners();
        }

        private void SetDefaultValues()
        {
            sourceICComponent = SourceComponent as ICComponentBase;
        }

        private void SetupListeners()
        {
            sourceICComponent.OnPhotonExitIC += ICComponentBase_OnPhotonExitIC;
        }

        private void RemoveListeners()
        {
            sourceICComponent.OnPhotonExitIC -= ICComponentBase_OnPhotonExitIC;
        }
        #endregion

        #region Generate Visuals
        private void GeneratePortVisuals()
        {
            GenerateInPorts();
            GenerateOutPorts();
        }

        private void GenerateInPorts()
        {
            GeneratePorts(SourceComponent.InPorts, inPortsHolder, inPortPrefab);
        }

        private void GenerateOutPorts()
        {
            GeneratePorts(SourceComponent.OutPorts, outPortsHolder, outPortPrefab);
        }

        private void GeneratePorts(ComponentPort[] ports, Transform holder, GameObject portPrefab)
        {
            for (int i = 0; i < ports.Length; i++)
                GeneratePortVisuals(ports[i], holder, portPrefab);
        }

        private void GeneratePortVisuals(ComponentPort port, Transform holder, GameObject portPrefab)
        {
            Transform portVisuals = Instantiate(portPrefab, holder).transform;

            portVisuals.position = GridUtils.GridPos2WorldPos(port.position, SourceComponent.HostGrid);
            RotateToLookAtOrientation(portVisuals, port.orientation);
        }
        #endregion

        #region Handle Events
        private void ICComponentBase_OnPhotonExitIC(Photon photon, ComponentPort port)
        {
            PhotonVisuals photonVisuals = Instantiate(photonPrefab);

            photonVisuals.SetSource(photon);
        }
        #endregion

        #region Handle Photon
        protected override void HandlePhoton(PhotonVisuals photon)
        {
            Destroy(photon.gameObject);
        }
        #endregion

        #region Handle Rotation
        protected override void HandleRotationChanged(Orientation orientation) => RotateToLookAtOrientation(visualsHolder, orientation);
        #endregion

        #region Handle Interaction
        // TEMP
        public void Interact()
        {
            GridManager.Instance.OpenGrid(sourceICComponent.InternalGrid);
        }
        #endregion
    }
}
