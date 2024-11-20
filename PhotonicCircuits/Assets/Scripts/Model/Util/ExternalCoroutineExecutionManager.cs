using Game.Data;
using SadUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ExternalCoroutineExecutionManager : Singleton<ExternalCoroutineExecutionManager>
    {
        private Dictionary<int, Coroutine> simulationRoutines;

        private int routineIdCounter;

        #region Awake / Destroy
        protected override void Awake()
        {
            SetDefaultValues();
            SetInstance(this);

            SetupListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void SetDefaultValues()
        {
            simulationRoutines = new();
            routineIdCounter = 0;
        }

        private void SetupListeners()
        {
            OpticComponent.OnStartProcessPhotonRoutine += OpticComponent_OnStartProcessPhotonRoutine;
            SimulationManager.OnSimulationStop += SimulationManager_OnSimulationStop;
        }

        private void RemoveListeners()
        {
            OpticComponent.OnStartProcessPhotonRoutine -= OpticComponent_OnStartProcessPhotonRoutine;
            SimulationManager.OnSimulationStop -= SimulationManager_OnSimulationStop;
        }
        #endregion

        #region Handle Events
        private void OpticComponent_OnStartProcessPhotonRoutine(IEnumerator routine) => StartSimulationCoroutine(routine);
        private void SimulationManager_OnSimulationStop() => StopSimulationRoutines();

        private void StopSimulationRoutines()
        {
            foreach (Coroutine routine in simulationRoutines.Values)
                StopCoroutine(routine);

            simulationRoutines.Clear();
        }
        #endregion

        #region Start Routines
        public void StartSimulationCoroutine(IEnumerator func)
        {
            Coroutine routine = StartCoroutine(SimulationRoutineCo(func, routineIdCounter));
            simulationRoutines.Add(routineIdCounter, routine);

            routineIdCounter++;
        }

        private IEnumerator SimulationRoutineCo(IEnumerator func, int routineId)
        {
            yield return func;
            simulationRoutines.Remove(routineId);
        }
        #endregion
    }
}
