using Game.Data;
using SadUtils;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class ComponentPaintManager : Singleton<ComponentPaintManager>
    {
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

        public void PaintComponent(Vector2Int position)
        {
            if (gridController.TryAddComponent(selectedComponent, position))
                return;

            // Show error
            Debug.Log("can't paint at " + position);
        }

        public void SelectComponent(ComponentPlaceDataSO placeData)
        {
            // Set paint control scheme.
            if (selectedComponent == null)
                PlayerInputManager.AddInputHandler(new GridComponentPaintInputHandler());

            selectedComponent = placeData;
        }

        private PlayerInputManager PlayerInputManager => PlayerInputManager.Instance;
    }
}
