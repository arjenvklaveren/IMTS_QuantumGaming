using Game.Data;
using SadUtils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class PhotonParticleVisuals : PhotonVisuals
    {
        protected override void SetDefaultValues(Photon photon)
        {
            base.SetDefaultValues(photon);
        }

        public override void SyncVisuals()
        {
            SyncPosition();
            SyncRotation();
            SyncColor();
        }

        #region Handle sprite visuals
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

        public override void StartMovement()
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            moveRoutine = StartCoroutine(MoveCo());
        }
        #endregion

        protected override void HandleEnterComponent(OpticComponent component)
        {
            base.HandleEnterComponent(component);
        }
        protected override void HandleExitComponent(OpticComponent component)
        {
            base.HandleExitComponent(component);
        }

        protected override void HandleDestroySource(bool storeVisuals = false)
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            Destroy(gameObject);
        }

        #region Overwrite Movement
        public void ForceMoveTile(Vector2 startPos, Vector2 endPos) => ForceMove(startPos, endPos, timeToTravelTile);
        public void ForceMoveHalfTile(Vector2 startPos, Vector2 endPos) => ForceMove(startPos, endPos, timeToTravelTile / 2f);
        public override void ForceMoveAlongNodes(Vector2[] nodes, ComponentPort outPort = null)
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            moveRoutine = StartCoroutine(ForceMoveAlongNodesCo(nodes, outPort));
        }

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
                yield return waitForEndOfFrame;
                timer -= Time.deltaTime;

                transform.position = Vector2.Lerp(startPos, endPos, 1f - (timer / duration));
            }

            transform.position = endPos;
        }

        private IEnumerator ForceMoveAlongNodesCo(Vector2[] nodes, ComponentPort outPort = null)
        {
            List<Vector2> nodeList = nodes.ToList();
            if(outPort != null) nodeList.Add(outPort.position); 

            for(int i = 0; i < nodeList.Count; i++)
            {
                float dist = Vector2.Distance(transform.position, nodeList[i]);
                float duration = timeToTravelTile * dist;
                yield return StartCoroutine(ForceMoveCo(transform.position, nodeList[i], duration));
            }
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
                    yield return waitForEndOfFrame;
                    timer -= Time.deltaTime;

                    transform.position = Vector2.Lerp(startPos, endPos, 1f - (timer / timeToTravelTile));
                }

                // Reset for next tile, carry extra progress!
                timer += timeToTravelTile;
            }
        }
        #endregion
    }
}
