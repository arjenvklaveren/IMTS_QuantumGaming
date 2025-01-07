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
    public class SerializationManager : Singleton<SerializationManager>
    {
        public const string SAVE_DIRECTORY = "/SaveData/Circuits";
        public const string BLUEPRINT_DIRECTORY = "/SaveData/Blueprints";

        private BlueprintSerializer blueprintSerializer;
        private SynchronizationContext mainThreadContext;

        private bool isSaving;

        #region Awake / Destroy
        protected override void Awake()
        {
            SetInstance(this);

            blueprintSerializer = new();
            mainThreadContext = SynchronizationContext.Current;
        }

        private void OnDestroy()
        {
            blueprintSerializer.Cancel();
        }
        #endregion

        public void SerializeProject() => SerializeProject(null);

        public void SerializeProject(Action completeCallback)
        {
            if (isSaving)
                return;

            IEnumerable<GridData> grids = new List<GridData>(GridManager.Instance.GetAllGrids());

            Task.Run(async () =>
            {
                await SerializeProjectAsync(grids);
                ExecuteOnMainThread(() => completeCallback?.Invoke());
            });
        }

        public void SerializeGrid(GridData grid, Action completeCallback)
        {
            Task.Run(async () =>
            {
                await SerializeGridAsync(grid);
                ExecuteOnMainThread(() => completeCallback?.Invoke());
            });
        }

        private async Task SerializeProjectAsync(IEnumerable<GridData> grids)
        {
            isSaving = true;

            foreach (GridData grid in grids)
                await SerializeGridAsync(grid);

            isSaving = false;
        }

        private async Task SerializeGridAsync(GridData grid)
        {
            await blueprintSerializer.SerializeBlueprints(grid);

            if (!grid.isIntegrated)
                SerializeProjectData(grid);
        }

        #region Serialize Grid Contents
        private void SerializeProjectData(GridData grid)
        {
            ProjectData projectData = new()
            {
                name = grid.gridName,
                gridData = grid,
                timeStamp = DateTime.Now
            };

            string jsonString = JsonConvert.SerializeObject(
                projectData,
                GetAllConverters());

            SaveFile(jsonString, grid.gridName);
        }

        private void SaveFile(string json, string projectName)
        {
            //Find file path.
            string filePath = GetFilePath(projectName);

            // Write to save file.
            using FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            // Write to file.
            using StreamWriter writer = new(fileStream);

            // Discard current file.
            fileStream.SetLength(0);

            // Write new Data.
            writer.Write(json);
        }
        #endregion

        #region Util
        public static JsonConverter[] GetAllConverters()
        {
            return new JsonConverter[]
            {
                // simple types
                new Vector2JsonConverter(),
                new Vector2IntJsonConverter(),

                // optics types
                new GridDataJsonConverter(),
                new OpticComponentJsonConverter(),
                new ComponentPortJsonConverter(),

                // blueprints
                new ICBlueprintDataJsonConverter(),
            };
        }

        public static string GetFilePath(string fileName, bool addExtension = true)
        {
            string filePath = $"{Application.dataPath}{SAVE_DIRECTORY}/{fileName}";

            if (addExtension)
                filePath += ".json";

            return filePath;
        }
        #endregion

        #region Execute On Main Thread
        private void ExecuteOnMainThread(Action action)
        {
            mainThreadContext.Post(_ => action?.Invoke(), null);
        }
        #endregion
    }
}
