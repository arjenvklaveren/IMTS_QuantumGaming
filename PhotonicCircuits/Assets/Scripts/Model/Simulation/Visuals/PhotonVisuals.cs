using Game.Data;
using SadUtils;
using System;
using System.Collections;
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

        private Coroutine moveRoutine;

        private OpticComponent hostComponent;
        private bool isInComponent;

        private float timeToTravelTile;

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

            // Set time to travel vars
            float tilesPerSecond = PhotonMovementManager.MoveSpeed;
            timeToTravelTile = 1f / tilesPerSecond;
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

        private void HandleEnterComponent(OpticComponent component)
        {
            if (isInComponent)
                return;

            isInComponent = true;
            hostComponent = component;

            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

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

            StartMovement();
        }

        private void HandleDestroy()
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            Destroy(gameObject);
        }
        #endregion

        #region Handle Sprite Visuals
        public void SyncVisuals()
        {
            SyncPosition();
            SyncRotation();
            SyncColor();
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

        public void StartMovement()
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            moveRoutine = StartCoroutine(MoveCo());
        }
        #endregion

        #region Overwrite Movement
        public void ForceMoveTile(Vector2 startPos, Vector2 endPos) => ForceMove(startPos, endPos, timeToTravelTile);
        public void ForceMoveHalfTile(Vector2 startPos, Vector2 endPos) => ForceMove(startPos, endPos, timeToTravelTile / 2f);

        public void ForceMove(Vector2 startPos, Vector2 endPos, float duration)
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            moveRoutine = StartCoroutine(ForceMoveCo(startPos, endPos, duration));
        }

        private IEnumerator ForceMoveCo(Vector2 startPos, Vector2 endPos, float duration)
        {
            float timer = duration;
            transform.position = startPos;

            while (timer > 0f)
            {
                yield return null;
                timer -= Time.deltaTime;

                transform.position = Vector2.Lerp(startPos, endPos, 1f - (timer / duration));
            }

            transform.position = endPos;
        }
        #endregion

        #region Move Loops
        private IEnumerator MoveCo()
        {
            float timer = timeToTravelTile;
            Vector2 moveStep = source.GetPropagationIntVector() * openGrid.spacing;

            while (true)
            {
                Vector2 startPos = transform.position;
                Vector2 endPos = startPos + moveStep;
                transform.position = Vector2.Lerp(startPos, endPos, 1f - (timer / timeToTravelTile));

                while (timer >= 0f)
                {
                    yield return null;
                    timer -= Time.deltaTime;

                    transform.position = Vector2.Lerp(startPos, endPos, 1f - (timer / timeToTravelTile));
                }

                // Reset for next tile, carry extra progress!
                timer += timeToTravelTile;
            }
        }
        #endregion

        PhotonMovementManager PhotonMovementManager => PhotonMovementManager.Instance;
    }
}
