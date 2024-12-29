using System.Collections;
using System.Collections.Generic;
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

        bool isOpen;

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
        }

        void RemoveListeners()
        {
            toggleButtonMain.onClick.RemoveListener(TogglePanel);
            toggleButtonBox.onClick.RemoveListener(TogglePanel);
        }
        #endregion


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
    }
}
