using Game.Data;
using Game.UI;
using SadUtils;
using SadUtils.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace Game
{
    public class SerializationManager : Singleton<SerializationManager>
    {
        public const string saveDirectory = "/SaveData/Circuits";
        public const string blueprintDirectory = "/SaveData/Blueprints";

        private bool isSaving;

        protected override void Awake()
        {
            SetInstance(this);
        }

        public void SerializeGrid(GridData grid)
        {
            if (isSaving)
                return;

            Task.Run(() => SerializeGridAsync(grid));
        }

        private async void SerializeGridAsync(GridData grid)
        {
            isSaving = true;
            await SerializeGridContentsAsync(grid);
            isSaving = false;
        }

        private async Task SerializeGridContentsAsync(GridData grid)
        {
            if (TryFindDirtyICComponents(grid, out List<ICComponentBase> dirtyICComponents))
                await SerializeDirtyICComponents(dirtyICComponents);
        }

        #region Serialize IC Components
        #region Find Dirty IC Components
        private bool TryFindDirtyICComponents(GridData grid, out List<ICComponentBase> dirtyICComponents)
        {
            dirtyICComponents = new();

            foreach (OpticComponent component in grid.placedComponents)
                if (TryCastToICComponent(component, out ICComponentBase icComponent))
                    if (icComponent.IsDirty)
                        dirtyICComponents.Add(icComponent);

            return dirtyICComponents.Count > 0;
        }

        private bool TryCastToICComponent(OpticComponent component, out ICComponentBase icComponent)
        {
            icComponent = null;
            bool isICComponent = IsICType(component.Type);

            if (isICComponent)
                icComponent = component as ICComponentBase;

            return isICComponent;
        }

        private bool IsICType(OpticComponentType type)
        {
            int typeId = (int)type;
            return typeId >= 100 && typeId < 200;
        }
        #endregion

        #region Serialize Dirty IC Components
        private async Task SerializeDirtyICComponents(List<ICComponentBase> dirtyICComponents)
        {
            foreach (ICComponentBase dirtyICComponent in dirtyICComponents)
                await SerializeDirtyICComponent(dirtyICComponent);
        }

        private async Task SerializeDirtyICComponent(ICComponentBase icComponent)
        {
            // Show unsaved changes prompt
            bool shouldEndProcess = await HandleUnsavedChangesPrompt(icComponent);

            if (shouldEndProcess)
                return;

            if (IsNewBlueprint(icComponent))
            {
                SaveNewBlueprint(icComponent);
                return;
            }
        }

        #region Unsaved Changes
        private async Task<bool> HandleUnsavedChangesPrompt(ICComponentBase icComponent)
        {
            bool shouldDiscard = await HandleUnsavedChangesPromptDisplay(icComponent);

            if (shouldDiscard)
                await DiscardChanges(icComponent);

            return shouldDiscard;
        }

        private async Task<bool> HandleUnsavedChangesPromptDisplay(ICComponentBase icComponent)
        {
            bool? shouldDiscard = null;

            void Discard() => shouldDiscard = true;
            void SaveChanges() => shouldDiscard = false;

            PopupData popupData = GetUnsavedChangesPopupData(
                icComponent,
                Discard,
                SaveChanges);

            while (shouldDiscard == null)
                await Task.Delay(100);

            return (bool)shouldDiscard;
        }

        private async Task DiscardChanges(ICComponentBase icComponent)
        {
            if (ICBlueprintManager.TryGetBlueprintData(icComponent.InternalGrid.gridName, out ICBlueprintData data))
                // Reset component back to blueprint state
                icComponent.SyncToBlueprint(data);

            else
                // No blueprint exists, remove component
                await RemoveComponent(icComponent);
        }

        private async Task RemoveComponent(ICComponentBase icComponent)
        {
            await HandleComponentRemovePrompt(icComponent);

            GridManager.Instance.GridController.TryRemoveComponent(icComponent);
        }

        private async Task HandleComponentRemovePrompt(ICComponentBase icComponent)
        {
            bool gotResponse = false;

            void HandleResponse() => gotResponse = true;

            PopupData popupData = GetComponentRemovePopupData(icComponent, HandleResponse);

            while (!gotResponse)
                await Task.Delay(100);
        }
        #endregion

        #region New Blueprint
        private bool IsNewBlueprint(ICComponentBase icComponent)
        {
            string blueprintName = icComponent.InternalGrid.gridName;

            return !ICBlueprintManager.DoesBlueprintExist(blueprintName);
        }

        private void SaveNewBlueprint(ICComponentBase icComponent)
        {
            ICBlueprintData data = new()
            {
                type = icComponent.Type,
                internalGrid = new(icComponent.InternalGrid)
            };

            ICBlueprintManager.SaveBlueprint(data);
        }
        #endregion

        #region Overwrite File
        private async Task HandleOverwriteFilePrompt(ICComponentBase icComponent)
        {
            bool shouldOverwriteFile = await HandleOverwriteFilePromptDisplay(icComponent);

            if (shouldOverwriteFile)
                OverwriteBlueprint(icComponent);
            //TODO create new file logic
        }

        private async Task<bool> HandleOverwriteFilePromptDisplay(ICComponentBase icComponent)
        {
            bool? shouldOverwrite = null;

            void HandleOverwrite() => shouldOverwrite = true;
            void HandleNewSave() => shouldOverwrite = false;

            PopupData popupData = GetOverwriteFilePopupData(icComponent, HandleOverwrite, HandleNewSave);
            PopupManager.ShowPopup(popupData);

            while (shouldOverwrite == null)
                await Task.Delay(100);

            return (bool)shouldOverwrite;
        }

        private void OverwriteBlueprint(ICComponentBase icComponent)
        {
            string blueprintName = icComponent.InternalGrid.gridName;

            if (!ICBlueprintManager.TryGetBlueprintData(blueprintName, out ICBlueprintData data))
                return;

            ICBlueprintManager.SaveBlueprint(data);
        }
        #endregion
        #endregion
        #endregion

        #region Popups
        private PopupData GetUnsavedChangesPopupData(ICComponentBase component, Action discardCallback, Action saveCallback)
        {
            string title = $"{component.InternalGrid.gridName} contains unsaved changes";
            PopupButtonData[] buttons = new PopupButtonData[2]
            {
                new PopupTextButtonData(discardCallback, "Discard Changes"),
                new PopupTextButtonData(saveCallback, "Save Changes")
            };

            return new PopupFactory()
                .AddTitle(title)
                .AddButtons(buttons)
                .Build();
        }

        private PopupData GetComponentRemovePopupData(ICComponentBase component, Action onResponse)
        {
            string title = $"{component.InternalGrid.gridName} will be removed";
            PopupContentData[] content = new PopupContentData[1]
            {
                new PopupTextContentData($"No blueprint state was found to reset {component.InternalGrid.gridName} to.\n" +
                $"Component will be removed.")
            };
            PopupButtonData[] button = new PopupButtonData[1]
            {
                new PopupTextButtonData(onResponse, "Ok")
            };

            return new PopupFactory()
                .AddTitle(title)
                .AddButtons(button)
                .Build();
        }

        private PopupData GetOverwriteFilePopupData(ICComponentBase component, Action overwriteCallback, Action newFileCallback)
        {
            string title = "Overwrite file?";
            PopupContentData[] content = new PopupContentData[1]
            {
                new PopupTextContentData($"Blueprint with name \"{component.InternalGrid.gridName}\" already exists.\n" +
                $"Overwrite blueprint or create a new blueprint?")
            };
            PopupButtonData[] buttons = new PopupButtonData[2]
            {
                new PopupTextButtonData(overwriteCallback, "Overwrite"),
                new PopupTextButtonData(newFileCallback, "Create New File")
            };

            return new PopupFactory()
                .AddTitle(title)
                .AddContents(content)
                .AddButtons(buttons)
                .Build();
        }
        #endregion

        //public void SerializeGrid(GridData grid)
        //{
        //    if (isSaving)
        //        return;

        //    isSaving = true;

        //    SaveGridContents(grid);
        //}

        //#region Serialize Grid
        //private async void SaveGridContents(GridData grid)
        //{
        //    Debug.Log("Saving file...");
        //    System.Diagnostics.Stopwatch timer = new();
        //    timer.Start();

        //    await SerializeAsync(grid);

        //    isSaving = false;

        //    Debug.Log($"Finished saving file in {timer.ElapsedMilliseconds}ms!");
        //    timer.Stop();
        //}

        //private Task SerializeAsync(GridData grid)
        //{
        //    return Task.Run(() =>
        //    {
        //        string jsonString = JsonConvert.SerializeObject(
        //        grid,
        //        GetAllConverters());

        //        SaveFile(jsonString, grid.gridName);
        //    });
        //}

        //private void SaveFile(string json, string gridName)
        //{
        //    // Find file path.
        //    string filePath = GetFilePath(gridName);

        //    // Write to save file.
        //    using FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        //    // Write to file.
        //    using StreamWriter writer = new(fileStream);

        //    // Discard current file.
        //    fileStream.SetLength(0);

        //    // Write new Data.
        //    writer.Write(json);
        //}
        //#endregion

        //#region Save Integrated Circuits
        //#region Find Dirty IC Components
        //private bool TryGetDirtyICComponents(GridData grid, out ICComponentBase[] dirtyICComponents)
        //{
        //    List<ICComponentBase> icComponents = new();

        //    foreach (OpticComponent opticComponent in grid.placedComponents)
        //    {
        //        if (!TryGetICComponent(opticComponent, out ICComponentBase icComponent))
        //            continue;

        //        if (icComponent.IsDirty)
        //            icComponents.Add(icComponent);
        //    }

        //    dirtyICComponents = icComponents.ToArray();
        //    return dirtyICComponents.Length > 0;
        //}

        //private bool TryGetICComponent(OpticComponent component, out ICComponentBase icComponent)
        //{
        //    icComponent = null;
        //    bool isICComponent = IsICComponentType(component.Type);

        //    if (isICComponent)
        //        icComponent = component as ICComponentBase;

        //    return isICComponent;
        //}

        //private bool IsICComponentType(OpticComponentType type)
        //{
        //    int typeId = (int)type;

        //    return typeId >= 100 && typeId < 200;
        //}
        //#endregion

        //#region Save IC Components
        //private void SaveICComponents(ICComponentBase[] dirtyICComponents)
        //{

        //}
        //#endregion
        //#endregion

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
            };
        }

        public static string GetFilePath(string fileName, bool addExtension = true)
        {
            string filePath = $"{Application.dataPath}{saveDirectory}/{fileName}";

            if (addExtension)
                filePath += ".json";

            return filePath;
        }
        #endregion

        // TEST
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
                SerializeGrid(GridManager.Instance.GetActiveGrid());
        }

        private PopupManager PopupManager => PopupManager.Instance;
        private ICBlueprintManager ICBlueprintManager => ICBlueprintManager.Instance;
    }
}
