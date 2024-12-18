using Game.Data;
using SadUtils.UI;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class BlueprintSerializer
    {
        private const int DELAY_TIME_STEP = 100;

        private enum UnsavedChangesResponse { Discard, Overwrite }
        private enum NewFileResponse { Submit, Cancel }
        private enum NameExistsResponse { Overwrite, Cancel }

        private readonly SynchronizationContext mainThreadContext;
        private bool isCanceled;

        public BlueprintSerializer()
        {
            mainThreadContext = SynchronizationContext.Current;
        }

        public async Task SerializeBlueprints(GridData grid)
        {
            List<ICComponentBase> dirtyICComponents = FindDirtyICComponents(grid);

            foreach (ICComponentBase dirtyICComponent in dirtyICComponents)
                await SaveDirtyICComponent(dirtyICComponent);
        }

        #region Find Dirty ICs
        private List<ICComponentBase> FindDirtyICComponents(GridData grid)
        {
            List<ICComponentBase> dirtyComponents = new();

            foreach (OpticComponent component in grid.placedComponents)
                if (TryGetICComponent(component, out ICComponentBase icComponent))
                    if (icComponent.IsDirty)
                        dirtyComponents.Add(icComponent);

            return dirtyComponents;
        }

        private bool TryGetICComponent(OpticComponent component, out ICComponentBase icComponent)
        {
            icComponent = null;

            bool isICComponent = IsICComponentType(component.Type);

            if (isICComponent)
                icComponent = component as ICComponentBase;

            return isICComponent;
        }

        private bool IsICComponentType(OpticComponentType type)
        {
            int typeId = (int)type;
            return typeId >= 100 && typeId < 200;
        }
        #endregion

        #region Save Dirty ICs
        private async Task SaveDirtyICComponent(ICComponentBase component)
        {
            UnsavedChangesResponse response = await HandleUnsavedChangesPopup(component);

            await HandleUnsavedChangesResponse(component, response);
        }

        #region Unsaved Changes
        private async Task<UnsavedChangesResponse> HandleUnsavedChangesPopup(ICComponentBase component)
        {
            UnsavedChangesResponse? popupResponse = null;

            void Overwrite() { popupResponse = UnsavedChangesResponse.Overwrite; }
            void Discard() { popupResponse = UnsavedChangesResponse.Discard; }

            PopupData popupData = GetUnsavedChangesPopupData(
                component,
                Overwrite,
                Discard);

            ExecuteOnMainThread(() => PopupManager.ShowPopup(popupData));

            while (popupResponse == null)
            {
                if (isCanceled)
                    break;

                await Task.Delay(DELAY_TIME_STEP);
            }

            return (UnsavedChangesResponse)popupResponse;
        }

        private async Task HandleUnsavedChangesResponse(ICComponentBase component, UnsavedChangesResponse response)
        {
            switch (response)
            {
                case UnsavedChangesResponse.Discard:
                    ExecuteOnMainThread(() => HandleDiscardChangesResponse(component));
                    break;

                case UnsavedChangesResponse.Overwrite:
                    await HandleOverwriteResponse(component);
                    break;
            }
        }
        #endregion

        #region Discard Changes
        private void HandleDiscardChangesResponse(ICComponentBase component)
        {
            if (!ICBlueprintManager.TryGetBlueprintData(component.InternalGrid.gridName, out ICBlueprintData data))
                return;

            // Reset Component to Blueprint state
            component.SyncToBlueprint(data);

            if (GridManager.GetActiveGrid().gridName != data.Name)
                return;

            // If currently shown grid, refresh grid
            GridManager.CloseActiveGrid();
            GridManager.OpenGrid(component.InternalGrid);
        }
        #endregion

        #region Overwrite Blueprint File
        private async Task HandleOverwriteResponse(ICComponentBase component)
        {
            ICBlueprintData newData = new(
                component.containedBlueprints,
                component.InternalGrid,
                component.Type);

            await ICBlueprintManager.SaveBlueprint(newData);
        }
        #endregion
        #endregion

        #region Popup Data
        private PopupData GetUnsavedChangesPopupData(
            ICComponentBase component,
            Action overWriteCallback,
            Action discardCallback)
        {
            string title = $"{component.InternalGrid.gridName} contains unsaved changes";

            PopupTextContentData textContent = new("Would you like to save these changes?");

            PopupTextButtonData[] buttons = new PopupTextButtonData[2]
            {
                new(overWriteCallback, "Overwrite Blueprint"),
                new(discardCallback, "Discard Changes")
            };

            return new PopupFactory()
                .AddTitle(title)
                .AddContents(textContent)
                .AddButtons(buttons)
                .Build();
        }

        private PopupData GetNewFilePopupData(
            Action<string> formContentChangedCallback,
            Action submitCallback,
            Action cancelCallback)
        {
            string title = "Create new blueprint";

            PopupContentData[] contents = new PopupContentData[2]
            {
                new PopupTextContentData("Name the new blueprint"),
                new PopupTextFormContentData(formContentChangedCallback, "Name")
            };

            PopupTextButtonData[] buttons = new PopupTextButtonData[2]
            {
                new(submitCallback, "Submit"),
                new(cancelCallback, "Cancel")
            };

            return new PopupFactory()
                .AddTitle(title)
                .AddContents(contents)
                .AddButtons(buttons)
                .Build();
        }

        private PopupData GetNameExistsPopupData(
            string name,
            Action overwriteCallback,
            Action cancelCallback)
        {
            string title = "Name already exists";

            PopupTextContentData content = new($"A blueprint with the name \"{name}\" already exists.\n" +
                $"Would you like to overwrite the existing blueprint?");

            PopupTextButtonData[] buttons = new PopupTextButtonData[2]
            {
                new(overwriteCallback, "Overwrite blueprint"),
                new(cancelCallback, "Cancel")
            };

            return new PopupFactory()
                .AddTitle(title)
                .AddContents(content)
                .AddButtons(buttons)
                .Build();
        }
        #endregion

        public void Cancel()
        {
            isCanceled = true;
        }

        private void ExecuteOnMainThread(Action action)
        {
            mainThreadContext.Post(_ => action?.Invoke(), null);
        }

        private PopupManager PopupManager => PopupManager.Instance;
        private GridManager GridManager => GridManager.Instance;
        private ICBlueprintManager ICBlueprintManager => ICBlueprintManager.Instance;
    }
}
