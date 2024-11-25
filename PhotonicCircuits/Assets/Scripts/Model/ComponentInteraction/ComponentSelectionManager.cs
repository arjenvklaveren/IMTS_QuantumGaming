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

        protected override void Awake()
        {
            SetInstance(this);
        }

        public void SelectComponent(ComponentVisuals selected)
        {
            SelectedVisuals = selected;

            HasSelection = true;

            OnSelectedComponent?.Invoke(SelectedVisuals);
        }

        public void Deselect()
        {
            HasSelection = false;

            OnDeselect?.Invoke();
        }
    }
}
