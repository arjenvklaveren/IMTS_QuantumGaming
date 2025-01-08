using Game.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LoadProjectPanel : Panel
    {
        [Header("Refs")]
        [SerializeField] private RectTransform contentHolder;
        [SerializeField] private ProjectListItem listItemPrefab;

        #region Awake
        private void Awake()
        {
            SetDefaultValues();
        }

        private IEnumerator Start()
        {
            yield return DeserializationManager.WaitForInstance;

            if (DeserializationManager.FinishedLoadingData)
                GenerateListElements();
            else
                SetupDeserializeListener();
        }

        private void SetDefaultValues()
        {
            gameObject.SetActive(false);
        }

        private void SetupDeserializeListener()
        {
            DeserializationManager.OnProjectDataLoaded += HandleDeserializeFinished;
        }
        #endregion

        #region Generate UI Elements
        private void HandleDeserializeFinished()
        {
            DeserializationManager.OnProjectDataLoaded -= HandleDeserializeFinished;
            GenerateListElements();
        }

        private void GenerateListElements()
        {
            Dictionary<string, ProjectData> cachedProjectData = DeserializationManager.CachedProjectData;

            foreach (KeyValuePair<string, ProjectData> pair in cachedProjectData)
                GenerateListElement(pair.Value, pair.Key);

            LayoutRebuilder.ForceRebuildLayoutImmediate(contentHolder);
        }

        private void GenerateListElement(ProjectData projectData, string filePath)
        {
            ProjectListItem listItem = Instantiate(listItemPrefab, contentHolder);

            listItem.Init(
                projectData,
                filePath);

            listItem.OnLoadProjectPressed += HandleLoadProject;
        }
        #endregion

        #region Handle Load Project
        private void HandleLoadProject(ProjectData projectData)
        {
            // load project
            GridManager.Instance.OpenProject(projectData.gridData);

            // close menu
            gameObject.SetActive(false);
        }
        #endregion

        private DeserializationManager DeserializationManager => DeserializationManager.Instance;
    }
}
