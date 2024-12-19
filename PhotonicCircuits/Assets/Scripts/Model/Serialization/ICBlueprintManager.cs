using Game.Data;
using SadUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class ICBlueprintManager : Singleton<ICBlueprintManager>
    {
        public Action<ICBlueprintData> OnBlueprintUpdated;

        private Dictionary<string, ICBlueprintData> loadedBlueprints;

        private string blueprintDirectory;

        #region Awake
        protected override void Awake()
        {
            if (!HasInstance)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }

            SetInstance(this);
            SetDefaultValues();
        }

        private void SetDefaultValues()
        {
            loadedBlueprints = new();
            blueprintDirectory = $"{Application.dataPath}{SerializationManager.blueprintDirectory}/";
        }

        private IEnumerator Start()
        {
            yield return SerializationManager.HasInstance;
            Task.Run(() => LoadBlueprintsAsync());
        }
        #endregion

        #region Load Blueprints
        private async Task LoadBlueprintsAsync()
        {
            string[] filePaths = GetAllBlueprintFilePaths();

            foreach (string filePath in filePaths)
                await LoadBlueprintFileAsync(filePath);
        }

        private string[] GetAllBlueprintFilePaths()
        {
            return Directory.GetFiles(blueprintDirectory, "*.json");
        }

        private async Task LoadBlueprintFileAsync(string filePath)
        {
            string fileContents = await LoadFileContentsAsync(filePath);

            ICBlueprintData blueprintData = JsonConvert.DeserializeObject<ICBlueprintData>(
                fileContents,
                SerializationManager.GetAllConverters());

            loadedBlueprints.Add(blueprintData.Name, blueprintData);
        }

        private async Task<string> LoadFileContentsAsync(string filePath)
        {
            using FileStream fileStream = File.OpenRead(filePath);
            using StreamReader reader = new(fileStream);

            return await reader.ReadToEndAsync();
        }
        #endregion

        #region Save Blueprints
        public async Task SaveBlueprint(ICBlueprintData data)
        {
            if (loadedBlueprints.ContainsKey(data.Name))
                OverwriteBlueprintData(data);
            else
                loadedBlueprints.Add(data.Name, data);

            await Task.Run(() => OverwriteSaveFile(data));
        }

        private void OverwriteBlueprintData(ICBlueprintData data)
        {
            loadedBlueprints[data.Name] = data;
            OnBlueprintUpdated?.Invoke(data);
        }

        private void OverwriteSaveFile(ICBlueprintData data)
        {
            string json = JsonConvert.SerializeObject(
                data,
                SerializationManager.GetAllConverters());

            WriteToFile(json, data.Name);
        }

        private void WriteToFile(string json, string fileName)
        {
            // Compile file path
            string filePath = $"{blueprintDirectory}{fileName}.json";

            // Write to save file.
            using FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            // Write to file.
            using StreamWriter writer = new(fileStream);

            // Discard current file
            fileStream.SetLength(0);

            // Write data
            writer.Write(json);
        }
        #endregion

        #region Get Blueprint Data
        public bool TryGetBlueprintData(string name, out ICBlueprintData data)
        {
            data = null;

            bool exists = loadedBlueprints.ContainsKey(name);

            if (exists)
                data = loadedBlueprints[name];

            return exists;
        }

        public List<string> GetAllBlueprintNames()
        {
            return new List<string>(loadedBlueprints.Keys);
        }

        public bool DoesBlueprintExist(string name) => loadedBlueprints.ContainsKey(name);
        #endregion
    }
}
