using SadUtils;
using System;

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
        }

        public void TogglePause()
        {
            if (!isSimulating)
                return;

            isPaused = !isPaused;

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
        }
    }
}
