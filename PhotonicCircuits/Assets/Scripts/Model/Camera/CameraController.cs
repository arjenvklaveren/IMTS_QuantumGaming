using Game.Data;
using UnityEngine;

namespace Game
{
    public class CameraController : MonoBehaviour
    {
        private void Awake()
        {
            GridController.OnGridChanged += GridController_OnGridChanged;
        }

        private void OnDestroy()
        {
            GridController.OnGridChanged -= GridController_OnGridChanged;
        }

        #region Event Listeners
        private void GridController_OnGridChanged(GridData gridData)
        {
            Vector2 centerPos = ((gridData.size * gridData.spacing) / 2f) - (gridData.spacing / 2f);
            SetPosition(centerPos);
        }
        #endregion

        #region Util
        private void SetPosition(Vector2 position)
        {
            Vector3 newPosition = position;
            newPosition.z = transform.position.z;

            transform.position = newPosition;
        }
        #endregion
    }
}
