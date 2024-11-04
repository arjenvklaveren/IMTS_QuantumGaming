using Game.Data;
using SadUtils;
using UnityEngine;

namespace Game
{
    public class ComponentPaintManager : Singleton<ComponentPaintManager>
    {
        [SerializeField] private ComponentPlaceDataSO testSO;

        private ComponentPlaceDataSO selectedComponent;

        private GridController gridController;

        protected override void Awake()
        {
            selectedComponent = testSO;

            SetInstance(this);
        }

        private void Start()
        {
            gridController = GridManager.Instance.gridController;
        }

        public void PaintComponent(Vector2Int position)
        {
            if (gridController.TryAddComponent(selectedComponent, position))
                return;

            // Show error
        }

        public void SelectComponent(ComponentPlaceDataSO placeData)
        {
            selectedComponent = placeData;
        }
    }
}
