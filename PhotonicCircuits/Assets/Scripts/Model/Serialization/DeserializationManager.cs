using Game.Data;
using Newtonsoft.Json;
using SadUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Game
{
    public class DeserializationManager : Singleton<DeserializationManager>
    {
        public event Action OnProjectDataLoaded;

        // Stores project data per file path.
        public Dictionary<string, ProjectData> CachedProjectData { get; private set; }
        public bool FinishedLoadingData { get; private set; }

        private JsonConverter[] cachedCustomConverters;

        private SynchronizationContext mainThreadContext;

        #region Awake
        protected override void Awake()
        {
            SetDefaultValues();

            SetInstance(this);

            Task.Run(LoadProjectData);
        }

        private void SetDefaultValues()
        {
            CachedProjectData = new();
            FinishedLoadingData = false;

            cachedCustomConverters = SerializationManager.GetAllConverters();

            mainThreadContext = SynchronizationContext.Current;
        }
        #endregion

        #region Load Save Files
        private async Task LoadProjectData()
        {
            string projectFileDirectory = $"{Application.dataPath}{SerializationManager.SAVE_DIRECTORY}";

            string[] filePaths = Directory.GetFiles(projectFileDirectory, "*.json");

            foreach (string filePath in filePaths)
                await LoadSaveFileAsync(filePath);

            ExecuteOnMainThread(HandleFinishFileLoad);
        }

        private async Task LoadSaveFileAsync(string filePath)
        {
            string jsonContent = await LoadFileContentsAsync(filePath);

            try
            {
                ProjectData projectData = JsonConvert.DeserializeObject<ProjectData>(
                    jsonContent,
                    cachedCustomConverters);

                CachedProjectData.Add(filePath, projectData);
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

        #region Handle Loading Finished
        private void HandleFinishFileLoad()
        {
            FinishedLoadingData = true;

            OnProjectDataLoaded?.Invoke();
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
