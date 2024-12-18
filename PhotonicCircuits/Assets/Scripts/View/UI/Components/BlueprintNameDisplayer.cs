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
            SetName(component.sourceICComponent.InternalGrid.gridName);

            component.sourceICComponent.OnNameChanged += SetName;
        }

        private void OnDestroy()
        {
            component.sourceICComponent.OnNameChanged -= SetName;
        }

        private void SetName(string name)
        {
            label.text = name;
        }
    }
}
