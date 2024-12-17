using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Data;

namespace Game.Test
{
    public class RotateSelectedTest : MonoBehaviour
    {
        ComponentVisuals selected;

        private void Start()
        {
            ComponentSelectionManager.Instance.OnSelectedComponent += OnSelectComponent;
        }

        private void OnSelectComponent(ComponentVisuals visuals)
        {
            selected = visuals;
        }

        private void Update()
        {
            if(UnityEngine.Input.GetKeyDown(KeyCode.P))
            {
                GridManager.Instance.GridController.TryRotateComponentClockwise(selected.SourceComponent);
            }
        }
    }
}
