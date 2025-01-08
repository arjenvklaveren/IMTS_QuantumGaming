using Game.Data;
using System.Collections;
using UnityEngine;

namespace Game.Test
{
    public class GridTest : MonoBehaviour
    {
        [SerializeField] private Vector2 gridSpacing;
        [SerializeField] private Vector2Int gridSize;

        private IEnumerator Start()
        {
            yield return GridManager.WaitForInstance;
            yield return null;

            GridData data = new(
                "",
                gridSpacing,
                gridSize);

            GridManager.Instance.OpenGrid(data);
        }
    }
}
