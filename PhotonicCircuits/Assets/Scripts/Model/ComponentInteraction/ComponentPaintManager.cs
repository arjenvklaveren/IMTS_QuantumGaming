using Game.Data;
using SadUtils;
using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class ComponentPaintManager : Singleton<ComponentPaintManager>
    {
        public event Action<ComponentPlaceDataSO> onPlaceDataChanged;

        private ComponentPlaceDataSO selectedComponent;

        private GridController gridController;

        protected override void Awake()
        {
            SetInstance(this);
        }

        private IEnumerator Start()
        {
            yield return GridManager.WaitForInstance;
            gridController = GridManager.Instance.GridController;
        }

        #region Paint Component
        public void PaintComponent(Vector2Int position)
        {
            if (gridController.TryAddComponent(selectedComponent, position))
                return;

            // Show error
            Debug.Log($"can't paint at {position}");
        }
        #endregion

        #region Select Place Data
        public void SelectComponent(ComponentPlaceDataSO placeData)
        {
            // Set paint control scheme.
            if (selectedComponent == null)
                PlayerInputManager.AddInputHandler(new GridComponentPaintInputHandler());

            selectedComponent = placeData;
            onPlaceDataChanged?.Invoke(placeData);
        }

        public void SelectBlueprint(string blueprintName)
        {
            ICBlueprintPlaceDataSO blueprintPlaceData = ScriptableObject.CreateInstance<ICBlueprintPlaceDataSO>();
            blueprintPlaceData.SetBlueprintReference(blueprintName);

            SelectComponent(blueprintPlaceData);
        }
        #endregion

        private PlayerInputManager PlayerInputManager => PlayerInputManager.Instance;
    }
}
