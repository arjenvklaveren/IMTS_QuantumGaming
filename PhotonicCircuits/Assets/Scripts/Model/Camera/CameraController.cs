using Game.Data;
using UnityEngine;

namespace Game
{
    public class CameraController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed;
        [SerializeField] private float dragSensitivity;
        [SerializeField] private Vector2 zoomDragMultRange;

        [Header("Zoom Settings")]
        [SerializeField] private float zoomSensitivity;
        [SerializeField] private Vector2 zoomLimits;

        [Header("References")]
        [SerializeField] private Camera cam;

        private Vector2 lastInputDir;

        private float currentZoom;

        private void Awake()
        {
            SetDefaultValues();

            GridController.OnGridChanged += GridController_OnGridChanged;
        }

        private void SetDefaultValues()
        {
            currentZoom = cam.orthographicSize;
            UpdateZoom(0f);
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

        #region RecieveInput
        public void SetMoveDir(Vector2 moveDir)
        {
            lastInputDir = moveDir;
        }

        public void Drag(Vector2 dragDelta)
        {
            Vector2 dragDir = dragDelta * (-dragSensitivity * GetZoomDragMult() / 100f);

            transform.Translate(dragDir);
        }

        private float GetZoomDragMult()
        {
            float zoomRelative = (currentZoom - zoomLimits.x) / (zoomLimits.y - zoomLimits.x);

            float zoomMultMaxDif = zoomDragMultRange.y - zoomDragMultRange.x;
            return zoomDragMultRange.x + (zoomMultMaxDif * zoomRelative);
        }

        public void UpdateZoom(float zoomChange)
        {
            currentZoom += zoomChange * zoomSensitivity;
            currentZoom = Mathf.Clamp(currentZoom, zoomLimits.x, zoomLimits.y);

            cam.orthographicSize = currentZoom;
        }
        #endregion

        #region Update Loop
        private void Update()
        {
            Move();
        }

        #region Movement
        private void Move()
        {
            Vector2 toMove = lastInputDir * (Time.unscaledDeltaTime * moveSpeed);

            transform.Translate(toMove);
        }
        #endregion
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
