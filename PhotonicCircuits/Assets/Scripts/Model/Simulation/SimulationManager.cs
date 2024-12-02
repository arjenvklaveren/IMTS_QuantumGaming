using SadUtils;
using System;
using UnityEngine;

namespace Game
{
    public class SimulationManager : Singleton<SimulationManager>
    {
        public static event Action OnSimulationInitialize;
        public static event Action OnSimulationStart;

        public static event Action OnSimulationPaused;
        public static event Action OnSimulationResumed;

        public static event Action OnSimulationStop;

        private static bool isSimulating;
        private static bool isPaused;

        protected override void Awake()
        {
            SetInstance(this);
        }

        public void StartSimulation()
        {
            if (isSimulating)
                return;

            isSimulating = true;
            OnSimulationInitialize?.Invoke();
            OnSimulationStart?.Invoke();

            PlayerInputManager.AddInputHandler(
                new GridSimulationInputHandler());
        }

        public void TogglePause()
        {
            if (!isSimulating)
                return;

            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;

            if (isPaused)
                OnSimulationPaused?.Invoke();
            else
                OnSimulationResumed?.Invoke();
        }

        public void StopSimulation()
        {
            if (!isSimulating)
                return;

            if (isPaused)
                TogglePause();

            isSimulating = false;
            OnSimulationStop?.Invoke();

            PlayerInputManager.PopInputHandler();
        }

        public bool IsSimulating() { return isSimulating; }
        private PlayerInputManager PlayerInputManager => PlayerInputManager.Instance;
    }
}
