using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class BlueprintNameDisplayer : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private ICComponentVisuals component;
        [SerializeField] private TMP_Text label;

        private void Start()
        {
            component.sourceICComponent.InternalGrid.OnBlueprintNamed += Rename;

            Rename(component.sourceICComponent.InternalGrid.gridName);
        }

        private void OnDestroy()
        {
            component.sourceICComponent.InternalGrid.OnBlueprintNamed -= Rename;
        }

        private void Rename(string newName)
        {
            label.text = newName;
        }
    }
}
