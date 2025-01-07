using Game.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LoadProjectPanel : Panel
    {
        [Header("Refs")]
        [SerializeField] private RectTransform contentHolder;
        [SerializeField] private ProjectListItem listItemPrefab;

        // Stores project data per file path.
        private Dictionary<string, ProjectData> cachedProjectData;
        private JsonConverter[] cachedCustomConverters;

        private SynchronizationContext mainThreadContext;

        #region Awake
        private void Awake()
        {
            SetDefaultValues();

            Task.Run(LoadProjectData);
        }

        private void SetDefaultValues()
        {
            cachedProjectData = new();
            cachedCustomConverters = SerializationManager.GetAllConverters();

            mainThreadContext = SynchronizationContext.Current;

            gameObject.SetActive(false);
        }
        #endregion

        #region Load Project Data Async
        private async Task LoadProjectData()
        {
            string projectFileDirectory = $"{Application.dataPath}{SerializationManager.SAVE_DIRECTORY}";

            string[] filePaths = Directory.GetFiles(projectFileDirectory, "*.json");

            foreach (string filePath in filePaths)
                await LoadSaveFileAsync(filePath);

            ExecuteOnMainThread(GenerateListElements);
        }

        private async Task LoadSaveFileAsync(string filePath)
        {
            string jsonContent = await LoadFileContentsAsync(filePath);

            try
            {
                ProjectData projectData = JsonConvert.DeserializeObject<ProjectData>(
                    jsonContent,
                    cachedCustomConverters);

                cachedProjectData.Add(filePath, projectData);
            }
            catch (Exception e)
            {
                Debug.LogError($"An error occured while deserializing project save data!\n" +
                    $"Error: {e.Message}\n" +
                    $"File: {filePath}");
            }
        }

        private async Task<string> LoadFileContentsAsync(string filePath)
        {
            using FileStream fileStream = File.OpenRead(filePath);
            using StreamReader reader = new(fileStream);

            return await reader.ReadToEndAsync();
        }
        #endregion

        #region Generate UI Elements
        private void GenerateListElements()
        {
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

            // close menu
            gameObject.SetActive(false);
        }
        #endregion

        #region Util
        private void ExecuteOnMainThread(Action action)
        {
            mainThreadContext.Post(_ => action?.Invoke(), null);
        }
        #endregion
    }
}
