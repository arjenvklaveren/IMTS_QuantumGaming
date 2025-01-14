using Game.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class SimulationDataPanel : Panel
    {
        [Header("Toggling references")]
        [SerializeField] Button toggleButtonMain;
        [SerializeField] Button toggleButtonBox;
        [SerializeField] Animator animator;

        [Header("Object references")]
        [SerializeField] Transform dataBarHolder;
        [SerializeField] Transform scrollbarHolder;
        [SerializeField] TextMeshProUGUI sequencesText;
        [SerializeField] Button resetButton;

        [Header("Prefab references")]
        [SerializeField] private SimDataBarItem dataBarItemRef;

        bool isOpen;

        List<PhotonDetectorComponent> currentDetectors = new List<PhotonDetectorComponent>();
        List<PhotonSourceComponent> currentSources = new List<PhotonSourceComponent>();

        Dictionary<string, SimDataBarItem> simStates = new Dictionary<string, SimDataBarItem>();

        List<int> currentMeasureIndexes = new List<int>();
        int currentTotalStateMeasurements = 0;
        int currentMeasureCount = 0;

        #region Initialisation
        private void Start()
        {
            AddListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        void AddListeners()
        {
            toggleButtonMain.onClick.AddListener(TogglePanel);
            toggleButtonBox.onClick.AddListener(TogglePanel);
            resetButton.onClick.AddListener(ResetData);
            GridController.OnGridChanged += GridController_OnGridChanged;
            GridController.OnComponentAdded += GridController_OnAddComponent;
            GridController.OnComponentRemoved += GridController_OnRemoveComponent;
            MeasuringManager.Instance.OnMeasurePhoton += MeasuringManager_OnMeasurePhoton;
            SimulationManager.OnSimulationStart += SimulationManager_OnSimulationStart;
        }

        void RemoveListeners()
        {
            toggleButtonMain.onClick.RemoveListener(TogglePanel);
            toggleButtonBox.onClick.RemoveListener(TogglePanel);
            resetButton.onClick.RemoveListener(ResetData);
            GridController.OnGridChanged -= GridController_OnGridChanged;
            GridController.OnComponentAdded -= GridController_OnAddComponent;
            GridController.OnComponentRemoved -= GridController_OnRemoveComponent;
            MeasuringManager.Instance.OnMeasurePhoton -= MeasuringManager_OnMeasurePhoton;
            SimulationManager.OnSimulationStart += SimulationManager_OnSimulationStart;
        }
        #endregion

        #region Event handling
        void GridController_OnGridChanged(GridData grid)
        {
            StartCoroutine(GridChangeDelay());
        }
        IEnumerator GridChangeDelay()
        {
            yield return GridManager.Instance.WaitUntilGrid;
            SyncRelevantComponents();
            SyncStates();
        }

        void GridController_OnAddComponent(OpticComponent component) 
        {
            if (component is PhotonDetectorComponent) { currentDetectors.Add(component as PhotonDetectorComponent); SyncStates(); } 
            if (component is PhotonSingleSourceComponent) { currentSources.Add(component as PhotonSingleSourceComponent); SyncStates(); }  
            if (component is PhotonLaserSourceComponent) { currentSources.Add(component as PhotonLaserSourceComponent); SyncStates(); } 
        }

        void GridController_OnRemoveComponent(OpticComponent component)
        {
            if (component is PhotonDetectorComponent) { currentDetectors.Remove(component as PhotonDetectorComponent);  SyncStates(); } 
            if (component is PhotonSingleSourceComponent) { currentSources.Remove(component as PhotonSingleSourceComponent);  SyncStates(); } 
            if (component is PhotonLaserSourceComponent) { currentSources.Remove(component as PhotonLaserSourceComponent);  SyncStates(); } 
        }

        private void SimulationManager_OnSimulationStart()
        {
            currentMeasureIndexes.Clear();
            currentMeasureIndexes.AddRange(Enumerable.Repeat(-1, currentSources.Count));
            currentMeasureCount = 0;
        }

        void MeasuringManager_OnMeasurePhoton(Photon photon, int stateIdentifier)
        {
            currentMeasureCount++;
            int sourceIndex = GetSourceIndex(photon);
            currentMeasureIndexes[sourceIndex] = stateIdentifier;

            if (currentMeasureCount >= currentSources.Count)
            {
                currentTotalStateMeasurements++;
                sequencesText.text = "Total sequences: " + currentTotalStateMeasurements.ToString();
                SetDataBarMeasurementData();
                currentMeasureIndexes.Clear();
                currentMeasureCount = 0;

                SimulationManager.Instance.StopSimulation();
                SimulationManager.Instance.StartSimulation();
            }
        }
        #endregion

        #region State possibilites setup
        public void SyncRelevantComponents()
        {
            currentSources.Clear();
            foreach(OpticComponent component in GridManager.Instance.GetActiveGrid().placedComponents)
            {
                if (component is PhotonSingleSourceComponent) currentSources.Add(component as PhotonSingleSourceComponent); 
                if (component is PhotonLaserSourceComponent) currentSources.Add(component as PhotonLaserSourceComponent);
            }
            currentDetectors = GridManager.Instance.GetActiveGrid().placedComponents.OfType<PhotonDetectorComponent>().ToList();
        }

        public void SyncStates()
        {
            ClearStates();
            CalculateStates();
            ResetData();
        }
        private void CalculateStates()
        {
            int[] detectorStates = currentDetectors.Select(x => x.GetStateIdentifier()).ToArray();
            int width = currentSources.Count;
            currentMeasureIndexes = new List<int>(new int[width]);
            if (detectorStates.Length * width < 1 || detectorStates.Length * width > 256) return;

            List<string> stringStates = CartesianProduct(detectorStates, width);
            GenerateDataBars(stringStates);
        }

        //Credit to chatGTP
        List<string> CartesianProduct(int[] set, int width)
        {
            List<List<int>> sequences = new List<List<int>>();
            for (int i = 0; i < width; i++)
                sequences.Add(set.ToList());

            List<string> result = new List<string> { "" };

            foreach (List<int> sequence in sequences)
            {
                result = result.SelectMany(
                    acc => sequence.Select(item => acc + item.ToString())
                ).ToList();
            }

            return result;
        }

        private void GenerateDataBars(List<string> stringStates)
        {
            Vector3 scrollbarScale = scrollbarHolder.localScale;
            if (stringStates.Count <= 4) scrollbarScale.y = 0;
            else scrollbarScale.y = 1;
            scrollbarHolder.localScale = scrollbarScale;

            foreach (string stateStr in stringStates)
            {
                SimDataBarItem newBarItem = Instantiate(dataBarItemRef, dataBarHolder.transform);
                newBarItem.SetReferenceText(stateStr);
                simStates.Add(stateStr, newBarItem);
            }
        }

        private void ClearStates()
        { 
            simStates.Clear();
            DestroyDataBarItems();
        }

        private void DestroyDataBarItems()
        {
            foreach(Transform child in dataBarHolder.transform) 
            { 
                Destroy(child.gameObject); 
            }
        }

        #endregion

        #region Measuring management
            
        private int GetSourceIndex(Photon photon)
        {
            for(int i = 0; i < currentSources.Count; i++)
            {
                if (photon.GetUniqueSourceKey() == currentSources[i].GetUniqueSourceKey()) return i;
            }
            return -1;
        }

        private void SetDataBarMeasurementData()
        {
            string state = "";
            for(int i = 0; i < currentMeasureIndexes.Count; i++) 
            {
                state += currentMeasureIndexes[i].ToString();
            }
            if (!simStates.ContainsKey(state)) return;

            SimDataBarItem relDataBar = simStates[state];
            relDataBar.AddCount();

            UpdateDataBars();
        }

        public void UpdateDataBars()
        {
            foreach (KeyValuePair<string, SimDataBarItem> simState in simStates)
            {
                if (simState.Value.GetCount() == 0) continue;
                float dataBarValue =  (float)simState.Value.GetCount() / (float)currentTotalStateMeasurements;
                simState.Value.SetBarValue01(dataBarValue);
            }
        }
        #endregion

        void ResetData()
        {
            currentMeasureIndexes.Clear();
            currentMeasureIndexes.AddRange(Enumerable.Repeat(-1, currentSources.Count));
            currentMeasureCount = 0;
            currentTotalStateMeasurements = 0;

            sequencesText.text = "Total sequences: " + currentTotalStateMeasurements.ToString();
            foreach (KeyValuePair<string, SimDataBarItem> simState in simStates)
            {
                simState.Value.ResetCount();
                simState.Value.SetBarValue01(0);
            }
        }

        #region Panel toggling
        void TogglePanel()
        {
            if (AnimatorIsPlaying()) return;
            isOpen = !isOpen;
            if (isOpen) animator.Play("OpenSimulationDataBox");
            else animator.Play("CloseSimulationDataBox");
        }

        bool AnimatorIsPlaying()
        {
            AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
            return (currentState.normalizedTime < 1.0f && currentState.length > 0);
        }
        #endregion
    }
}
