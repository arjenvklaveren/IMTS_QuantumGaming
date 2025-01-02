using Game.Data;
using SadUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class PhotonVisuals : MonoBehaviour
    {
        [SerializeField] protected SpriteRenderer sprite;

        public static event Action<PhotonVisuals, OpticComponent> OnEnterComponent;

        public Photon source { get; protected set; }
        protected GridData openGrid;

        protected OpticComponent hostComponent;
        protected bool isInComponent;

        protected Coroutine moveRoutine;
        protected float timeToTravelTile;
        protected WaitForEndOfFrame waitForEndOfFrame;

        protected PhotonMovementManager PhotonMovementManager => PhotonMovementManager.Instance;

        #region Awake / Destroy
        public virtual void SetSource(Photon photon)
        {
            SetDefaultValues(photon);
            SetupListeners();

            SyncVisuals();
            StartMovement();
        }

        protected virtual void SetDefaultValues(Photon photon)
        {
            source = photon;

            // Cache opened grid for simulation
            openGrid = GridManager.Instance.GetActiveGrid();

            float tilesPerSecond = PhotonMovementManager.MoveSpeed;
            timeToTravelTile = 1f / tilesPerSecond;
            waitForEndOfFrame = new();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        protected void SetupListeners()
        {
            source.OnEnterComponent += Photon_OnEnterComponent;
            source.OnExitComponent += Photon_OnExitComponent;
            source.OnDestroy += Source_OnDestroy;
        }

        protected void RemoveListeners()
        {
            source.OnEnterComponent -= Photon_OnEnterComponent;
            source.OnExitComponent -= Photon_OnExitComponent;
            source.OnDestroy -= Source_OnDestroy;
        }
        #endregion

        #region Handle Events
        private void Photon_OnEnterComponent(OpticComponent component) => HandleEnterComponent(component);
        private void Photon_OnExitComponent(OpticComponent component) => HandleExitComponent(component);
        private void Source_OnDestroy(bool destroyVisuals = true) => HandleDestroySource(destroyVisuals);

        protected virtual void HandleEnterComponent(OpticComponent component)
        {
            if (isInComponent)
                return;

            isInComponent = true;
            hostComponent = component;

            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            OnEnterComponent?.Invoke(this, component);
        }

        protected virtual void HandleExitComponent(OpticComponent component)
        {
            if (!isInComponent)
                return;

            if (hostComponent != component)
                return;

            isInComponent = false;
            SyncVisuals();

            StartMovement();
        }
        #endregion

        public virtual void SyncVisuals() { }
        public virtual void StartMovement() { }
        public virtual void ForceMoveAlongNodes(List<Vector2> nodes, WaveguideNodeHandler nodeHandler)
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            moveRoutine = StartCoroutine(ForceMoveAlongNodesCo(nodes, nodeHandler));
        }
        protected virtual IEnumerator ForceMoveAlongNodesCo(List<Vector2> nodes, WaveguideNodeHandler nodeHandler) { yield break; }

        protected virtual void HandleDestroySource(bool storeVisuals = false) { }
        public void SetAsInComponent(OpticComponent component) { hostComponent = component; isInComponent = true; }
    }
}
