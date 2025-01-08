using Game.Data;
using Newtonsoft.Json;
using SadUtils;
using SadUtils.UI;
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
        private enum SetNamePopupResponse { Cancel, Submit }

        public const string SAVE_DIRECTORY = "/SaveData/Circuits";
        public const string BLUEPRINT_DIRECTORY = "/SaveData/Blueprints";

        private const int DELAY_TIME_STEP = 100;

        private BlueprintSerializer blueprintSerializer;
        private SynchronizationContext mainThreadContext;

        private bool isSaving;

        private bool isCanceled;

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
            isCanceled = true;
        }
        #endregion

        #region Serialize Project
        // This function is called by UI button
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
            {
                bool namedProject = await TryNameProject(grid);

                if (namedProject)
                    SerializeProjectData(grid);
            }
        }
        #endregion

        #region Try Name Project
        private async Task<bool> TryNameProject(GridData grid, string error = "")
        {
            if (!string.IsNullOrEmpty(grid.gridName))
                return true;

            Tuple<SetNamePopupResponse, string> response = await ShowSetNamePopup(error);

            return await HandleSetNameResponse(response.Item1, response.Item2, grid);
        }

        private async Task<Tuple<SetNamePopupResponse, string>> ShowSetNamePopup(string error = "")
        {
            SetNamePopupResponse? response = null;
            string submitData = "";

            void UpdateSubmitData(string data) => submitData = data;
            void Submit() => response = SetNamePopupResponse.Submit;
            void Cancel() => response = SetNamePopupResponse.Cancel;

            PopupData popupData = GetNameSetPopupData(
                UpdateSubmitData,
                Submit,
                Cancel,
                error);

            ExecuteOnMainThread(() => PopupManager.Instance.ShowPopup(popupData));

            while (response == null)
            {
                if (isCanceled)
                    break;

                await Task.Delay(DELAY_TIME_STEP);
            }

            return new((SetNamePopupResponse)response, submitData);
        }

        private async Task<bool> HandleSetNameResponse(SetNamePopupResponse response, string name, GridData grid)
        {
            switch (response)
            {
                case SetNamePopupResponse.Submit:
                    if (IsValidName(name, out string error))
                    {
                        grid.gridName = name;
                        // Notify UI elements of name change
                        ExecuteOnMainThread(() => GridManager.Instance.TriggerProjectRenamed(name));
                        return true;
                    }
                    else
                        return await TryNameProject(grid, error);

                case SetNamePopupResponse.Cancel:
                default:
                    return false;
            }
        }

        private bool IsValidName(string name, out string error)
        {
            error = "";

            if (string.IsNullOrEmpty(name))
            {
                error = "Name cannot be empty!";
                return false;
            }

            if (DeserializationManager.Instance.ProjectWithNameExists(name))
            {
                error = $"Project with name \"{name}\" already exists!";
                return false;
            }

            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                if (name.Contains(c))
                {
                    error = $"Project name cannot contain symbol \"{c}\"!";
                    return false;
                }
            }

            return true;
        }
        #endregion

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

        #region Popup Data
        private PopupData GetNameSetPopupData(
            Action<string> formContentChangedCallback,
            Action submitCallback,
            Action cancelCallback,
            string error = "")
        {
            string title = "Project has no name";

            List<PopupContentData> content = new()
            {
                new PopupTextContentData("Project cannot be saved without a name"),
                new PopupTextFormContentData(formContentChangedCallback, "My Project"),
            };

            // Insert error message if needed
            if (!string.IsNullOrEmpty(error))
                content.Insert(0, new PopupTextContentData($"<color=\"red\"><b>{error}</b></color>"));

            PopupTextButtonData[] buttons = new PopupTextButtonData[2]
            {
                new(submitCallback, "Sumbit"),
                new(cancelCallback, "Cancel"),
            };

            return new PopupFactory()
                .AddTitle(title)
                .AddContents(content)
                .AddButtons(buttons)
                .Build();
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
