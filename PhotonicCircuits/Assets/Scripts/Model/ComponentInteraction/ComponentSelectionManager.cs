using SadUtils;
using System;

namespace Game
{
    public class ComponentSelectionManager : Singleton<ComponentSelectionManager>
    {
        public event Action<ComponentVisuals> OnSelectedComponent;
        public event Action OnDeselect;

        public ComponentVisuals SelectedVisuals { get; private set; }

        public bool HasSelection { get; private set; }

        #region Awake / Destroy
        protected override void Awake()
        {
            SetInstance(this);
            SetupListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void SetupListeners()
        {
            GridController.OnGridChanged += GridController_OnGridChanged;
        }

        private void RemoveListeners()
        {
            GridController.OnGridChanged -= GridController_OnGridChanged;
        }
        #endregion

        #region Handle Events
        private void GridController_OnGridChanged(Data.GridData obj)
        {
            Deselect();
        }
        #endregion

        #region Manage Selection
        public void SelectComponent(ComponentVisuals selected)
        {
            if (SelectedVisuals != null) SelectedVisuals.SetOutlineState(false);

            SelectedVisuals = selected;

            HasSelection = true;

            OnSelectedComponent?.Invoke(SelectedVisuals);

            selected.SetOutlineState(true);

            // TEST
            if (IsICComponentType(selected.SourceComponent.Type))
                (selected as ICComponentVisuals).Interact();
        }

        private bool IsICComponentType(Data.OpticComponentType type)
        {
            int typeId = (int)type;
            return typeId >= 100 && typeId < 200;
        }

        public void Deselect()
        {
            if (SelectedVisuals != null) SelectedVisuals.SetOutlineState(false);
            HasSelection = false;
            SelectedVisuals = null;
            OnDeselect?.Invoke();
        }
        #endregion
    }
}
