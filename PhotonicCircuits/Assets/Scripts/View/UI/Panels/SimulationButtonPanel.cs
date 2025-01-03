using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class SimulationButtonPanel : Panel
    {
        [SerializeField] private GameObject startButtonObject;
        [SerializeField] private GameObject playButtonObject;
        [SerializeField] private GameObject pauseButtonObject;
        [SerializeField] private Animator gridOutlineAnimator; 

        #region Awake/destroy
        private void Start()
        {
            SetupListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        void SetupListeners()
        {
            SimulationManager.OnSimulationStart += SimulationManager_OnStart;
            SimulationManager.OnSimulationPaused += SimulationManager_OnPause;
            SimulationManager.OnSimulationStop += SimulationManager_OnStop;
            SimulationManager.OnSimulationResumed += SimulationManager_OnResume;
        }

        void RemoveListeners()
        {
            SimulationManager.OnSimulationStart -= SimulationManager_OnStart;
            SimulationManager.OnSimulationPaused -= SimulationManager_OnPause;
            SimulationManager.OnSimulationStop -= SimulationManager_OnStop;
            SimulationManager.OnSimulationResumed -= SimulationManager_OnResume;
        }
        #endregion

        #region Handle events
        void SimulationManager_OnStart() => SetPlayPause(true, true);
        void SimulationManager_OnStop() => SetPlayPause(false, true);
        void SimulationManager_OnResume() => SetPlayPause(true);
        void SimulationManager_OnPause() => SetPlayPause(false);

        void SetPlayPause(bool isPlaying, bool isMainToggle = false)
        {
            startButtonObject.SetActive(!isPlaying && isMainToggle);
            playButtonObject.SetActive(!isPlaying && !isMainToggle);
            pauseButtonObject.SetActive(isPlaying);
            SetOutlineAnimationState(isPlaying, isMainToggle);
        }

        void SetOutlineAnimationState(bool isPlaying, bool isMainToggle)
        {
            if (isMainToggle && isPlaying) gridOutlineAnimator.Play("GridOutlineBlink");
            if (isMainToggle && !isPlaying) gridOutlineAnimator.Play("GridOutlineDefault");
            if(!isMainToggle && !isPlaying) gridOutlineAnimator.Play("GridOutlineBlink", 0, 0.5f);
        }
        #endregion
    }
}
