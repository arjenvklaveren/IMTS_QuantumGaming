using Game.Data;
using SadUtils.UI;
using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class ICComponentVisuals : ComponentVisuals
    {
        [Space]
        [SerializeField] private Transform inPortsHolder;
        [SerializeField] private Transform outPortsHolder;

        [Header("Prefab References")]
        [SerializeField] private PhotonVisuals photonPrefab;
        [SerializeField] private GameObject inPortPrefab;
        [SerializeField] private GameObject outPortPrefab;

        public ICComponentBase sourceICComponent;

        #region Create / Destroy
        public override void SetSource(OpticComponent component)
        {
            base.SetSource(component);
            SetDefaultValues();
            GeneratePortVisuals();

            SetupListeners();

            TryHandleNewBlueprint();
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

        #region Handle Set Name
        private enum SetNameResponse { Submit, Cancel }

        private void TryHandleNewBlueprint()
        {
            if (!string.IsNullOrEmpty(sourceICComponent.InternalGrid.gridName))
                return;

            // IC Component is new blueprint, force user to set name
            UpdateInputHandler();
            HandleSetName();
        }

        #region Update Input Handler
        private void UpdateInputHandler()
        {
            PlayerInputManager.PopInputHandler();
        }
        #endregion

        private void HandleSetName(string error = "")
        {
            StartCoroutine(ShowSetNamePopupCo(error));
        }

        private IEnumerator ShowSetNamePopupCo(string error = "")
        {
            WaitForSeconds waitTimeStep = new(0.1f);

            SetNameResponse? response = null;
            string inputFieldContent = "";

            void OnEndEdit(string data) => inputFieldContent = data;
            void Submit() => response = SetNameResponse.Submit;
            void Cancel() => response = SetNameResponse.Cancel;

            PopupData popupData = GetSetNamePopupData(
                OnEndEdit,
                Submit,
                Cancel,
                error);

            PopupManager.Instance.ShowPopup(popupData);

            while (response == null)
                yield return waitTimeStep;

            HandleSetNameResponse((SetNameResponse)response, inputFieldContent);
        }

        private void HandleSetNameResponse(SetNameResponse response, string data)
        {
            if (response == SetNameResponse.Cancel)
                HandleCancelResponse();
            else
                HandleSubmitResponse(data);
        }

        private void HandleCancelResponse()
        {
            // Remove Component from grid
            GridController gridController = GridManager.Instance.GridController;

            gridController.TryRemoveComponent(SourceComponent);
        }

        private void HandleSubmitResponse(string name)
        {
            // Handle Invalid Name
            if (IsInvalidName(name))
            {
                HandleSetName("Name cannot contain invalid characters or be empty!");
                return;
            }

            // Name already exists check
            if (ICBlueprintManager.Instance.DoesBlueprintExist(name))
            {
                HandleSetName($"A blueprint with name {name} already exists!");
                return;
            }

            // Save as new blueprint
            sourceICComponent.SetName(name);
            ICBlueprintData newBlueprint = new(
                sourceICComponent.containedBlueprints,
                sourceICComponent.InternalGrid,
                sourceICComponent.Type);

            Task.Run(() => ICBlueprintManager.Instance.SaveBlueprint(newBlueprint));

            GridManager.Instance.GetActiveGrid().TriggerNameBlueprint(name);
        }

        private bool IsInvalidName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return true;

            // Check for invalid names
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char invalidChar in invalidChars)
                if (name.Contains(invalidChar))
                    return true;

            return false;
        }

        #region Get Popup Data
        private PopupData GetSetNamePopupData(
            Action<string> endEditCallback,
            Action SubmitCallback,
            Action CancelCallback,
            string error)
        {
            string title = "Set Blueprint Name";

            PopupTextFormContentData content = new(endEditCallback, "Name");

            PopupTextButtonData[] buttons = new PopupTextButtonData[2]
            {
                new(SubmitCallback, "Submit"),
                new(CancelCallback, "Cancel")
            };

            PopupFactory factory = new();

            // Handle error messages
            if (!string.IsNullOrEmpty(error))
            {
                PopupTextContentData errorContent = new($"<color=\"red\"><b>{error}</b></color>");
                factory.AddContents(errorContent);
            }

            return factory.AddTitle(title)
                .AddContents(content)
                .AddButtons(buttons)
                .Build();
        }
        #endregion
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

        private PlayerInputManager PlayerInputManager => PlayerInputManager.Instance;
    }
}
