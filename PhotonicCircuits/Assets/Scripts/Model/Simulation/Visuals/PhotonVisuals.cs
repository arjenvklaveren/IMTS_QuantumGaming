using Game.Data;
using SadUtils;
using System;
using UnityEngine;

namespace Game
{
    public class PhotonVisuals : MonoBehaviour
    {
        public static event Action<PhotonVisuals, OpticComponent> OnEnterComponent;

        [Header("External Components")]
        [SerializeField] private SpriteRenderer sprite;

        private Photon source;
        private GridData openGrid;

        private OpticComponent hostComponent;
        private bool isInComponent;

        private Vector2 moveDir;
        private Vector2 moveSpeed;

        #region Awake / Destroy
        public void SetSource(Photon photon)
        {
            SetDefaultValues(photon);
            SetupListeners();
        }

        private void SetDefaultValues(Photon photon)
        {
            source = photon;

            // Cache opened grid for simulation
            openGrid = GridManager.Instance.GetActiveGrid();

            // Cache move speed based on open grid
            float tilesPerSecond = PhotonMovementManager.Instance.moveSpeed;
            Vector2 tileSize = openGrid.spacing;

            moveSpeed = tilesPerSecond * tileSize;
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void SetupListeners()
        {
            source.OnEnterComponent += Photon_OnEnterComponent;
            source.OnExitComponent += Photon_OnExitComponent;

            SimulationManager.OnSimulationStop += SimulationManager_OnSimulationStop;
        }

        private void RemoveListeners()
        {
            source.OnEnterComponent -= Photon_OnEnterComponent;
            source.OnExitComponent -= Photon_OnExitComponent;

            SimulationManager.OnSimulationStop -= SimulationManager_OnSimulationStop;
        }
        #endregion

        #region Handle Events
        private void Photon_OnEnterComponent(OpticComponent component) => HandleEnterComponent(component);
        private void Photon_OnExitComponent(OpticComponent component) => HandleExitComponent(component);
        private void SimulationManager_OnSimulationStop() => HandleSimulationStop();

        private void HandleEnterComponent(OpticComponent component)
        {
            if (isInComponent)
                return;

            isInComponent = true;
            hostComponent = component;

            OnEnterComponent?.Invoke(this, component);
        }

        private void HandleExitComponent(OpticComponent component)
        {
            if (!isInComponent)
                return;

            if (hostComponent != component)
                return;

            isInComponent = false;
            SyncVisuals();
        }

        private void HandleSimulationStop()
        {
            Destroy(gameObject);
        }
        #endregion

        #region Handle Sprite Visuals
        public void SyncVisuals()
        {
            SyncPosition();
            SyncRotation();
            SyncColor();
            SyncMoveDir();
        }

        private void SyncPosition()
        {
            Vector2 newPosition = GridUtils.GridPos2WorldPos(source.GetPosition(), openGrid);
            transform.position = newPosition;
        }

        private void SyncRotation()
        {
            Vector3 orientation = source.GetPropagationVector();
            Vector3 lookAtTarget = transform.position + orientation;

            transform.rotation = LookAt2D.GetLookAtRotation(transform, lookAtTarget);
        }

        private void SyncColor()
        {
            sprite.color = source.GetColor();
        }

        private void SyncMoveDir()
        {
            moveDir = source.GetPropagationVector();
        }
        #endregion

        #region Update Loop
        private void Update()
        {
            if (isInComponent)
                return;

            Move();
        }

        private void Move()
        {
            Vector3 translation = moveDir * moveSpeed * Time.deltaTime;
            transform.Translate(translation);
        }
        #endregion
    }
}
