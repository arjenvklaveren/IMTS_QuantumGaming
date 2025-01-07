using Game.Data;
using System;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class ProjectListItem : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private TMP_Text nameField;
        [SerializeField] private TMP_Text filePathField;

        [Space]
        [SerializeField] private GameObject timeStampHolder;
        [SerializeField] private TMP_Text dateField;
        [SerializeField] private TMP_Text timeField;

        [Space]
        [SerializeField] private GameObject detailButtonsHolder;

        public event Action<ProjectData> OnLoadProjectPressed;

        private ProjectData refData;

        public void Init(
            ProjectData projectData,
            string filePath)
        {
            nameField.text = projectData.name;
            filePathField.text = filePath;

            dateField.text = projectData.timeStamp.ToShortDateString();
            timeField.text = projectData.timeStamp.ToShortTimeString();

            refData = projectData;

            ToggleHover(false);
        }

        #region Hover
        public void HandleHover()
        {
            if (detailButtonsHolder.activeSelf)
                return;

            ToggleHover(true);
        }

        public void HandleUnHover()
        {
            if (timeStampHolder.activeSelf)
                return;

            ToggleHover(false);
        }

        private void ToggleHover(bool state)
        {
            timeStampHolder.SetActive(!state);
            detailButtonsHolder.SetActive(state);
        }
        #endregion

        public void Btn_LoadProject()
        {
            OnLoadProjectPressed?.Invoke(refData);
        }

        private void OnDisable()
        {
            ToggleHover(false);
        }
    }
}
