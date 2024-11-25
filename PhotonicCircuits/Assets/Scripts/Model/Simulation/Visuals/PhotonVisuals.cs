using Game.Data;
using SadUtils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

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
        private bool isInComponent = true;

        private Vector2 moveDir;
        private Vector2 moveSpeed;

        private bool lateStopCoroutine = false;

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
            float tilesPerSecond = PhotonMovementManager.Instance.MoveSpeed;
            Vector2 tileSize = openGrid.spacing;

            moveSpeed = tilesPerSecond * tileSize;

            moveCoroutine = Move();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        private void SetupListeners()
        {
            source.OnEnterComponent += Photon_OnEnterComponent;
            source.OnExitComponent += Photon_OnExitComponent;
            source.OnDestroy += Source_OnDestroy;
        }

        private void RemoveListeners()
        {
            source.OnEnterComponent -= Photon_OnEnterComponent;
            source.OnExitComponent -= Photon_OnExitComponent;
            source.OnDestroy -= Source_OnDestroy;
        }
        #endregion

        #region Handle Events
        private void Photon_OnEnterComponent(OpticComponent component) => HandleEnterComponent(component);
        private void Photon_OnExitComponent(OpticComponent component) => HandleExitComponent(component);
        private void Source_OnDestroy() => HandleDestroy();

        IEnumerator moveCoroutine = null;

        private void HandleEnterComponent(OpticComponent component)
        {
            if (isInComponent)
                return;

            isInComponent = true;
            hostComponent = component;

            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
                lateStopCoroutine = true;
            }

            SyncVisuals();

            OnEnterComponent?.Invoke(this, component);
        }

        private void HandleExitComponent(OpticComponent component)
        {
            if (!isInComponent)
                return;

            if (hostComponent != component && hostComponent != null)
                return;


            isInComponent = false;

            SyncVisuals();
            StartCoroutine(moveCoroutine);
        }

        private void HandleDestroy()
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

            sprite.transform.rotation = LookAt2D.GetLookAtRotation(transform, lookAtTarget);
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
            //MoveUpdate();
        }

        void MoveUpdate()
        {
            Vector3 translation = moveDir * moveSpeed * Time.deltaTime;
            transform.Translate(translation);
        }

        private IEnumerator Move()
        {
            if (isInComponent) yield break;

            Vector2 startPos = new Vector2(transform.position.x, transform.position.y);
            Vector2 targetPos = startPos + (moveDir.normalized);

            Vector2 timeSteps = new Vector2(1.0f / moveSpeed.x, 1.0f / moveSpeed.y);
            Vector2 elapsedTimes = new Vector2(0.0f, 0.0f);

            while (elapsedTimes.x < 1f || elapsedTimes.y < 1f)
            {
                if (lateStopCoroutine)
                {
                    lateStopCoroutine = false;
                    break;
                }

                elapsedTimes += new Vector2(Time.deltaTime / timeSteps.x, Time.deltaTime / timeSteps.y);
                if (elapsedTimes.x > 1.0f) elapsedTimes.x = 1.0f;
                if (elapsedTimes.y > 1.0f) elapsedTimes.y = 1.0f;

                transform.position = new Vector3(Mathf.Lerp(startPos.x, targetPos.x, elapsedTimes.x), Mathf.Lerp(startPos.y, targetPos.y, elapsedTimes.y), 0);
                yield return null;
            }

            if (!isInComponent)
            {
                moveCoroutine = Move();
                StartCoroutine(moveCoroutine);
            }
        }
        #endregion
    }
}
