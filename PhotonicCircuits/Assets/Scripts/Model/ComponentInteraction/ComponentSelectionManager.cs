using SadUtils;
using System;
using System.Diagnostics;

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
            SelectedVisuals = selected;

            HasSelection = true;

            OnSelectedComponent?.Invoke(SelectedVisuals);

            // TEST
            if (selected.SourceComponent.Type == Data.OpticComponentType.IC1x1 ||
                selected.SourceComponent.Type == Data.OpticComponentType.IC2x2)
                (selected as ICComponentVisuals).Interact();

            if (selected.SourceComponent.Type == Data.OpticComponentType.Source)
                GridManager.Instance.GridController.TryRotateComponentClockwise(selected.SourceComponent);
        }

        public void Deselect()
        {
            HasSelection = false;

            OnDeselect?.Invoke();
        }
        #endregion
    }
}
