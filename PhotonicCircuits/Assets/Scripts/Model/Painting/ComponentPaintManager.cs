using Game.Data;
using SadUtils;
using System.Collections;
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

        private IEnumerator Start()
        {
            yield return GridManager.WaitForInstance;
            gridController = GridManager.Instance.gridController;
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
            selectedComponent = placeData;
        }
    }
}
