using Game.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    public class PhotonBeamVisuals : PhotonVisuals
    {
        [SerializeField] private Transform visualsHolder;

        private SpriteRenderer currentDrawSprite;
        private Orientation currentOrientation;
        private float currentAmplitude;

        protected override void SetDefaultValues(Photon photon)
        {
            base.SetDefaultValues(photon);

            currentDrawSprite.transform.position = GridUtils.GridPos2WorldPos(source.GetPosition(), openGrid);
            currentOrientation = photon.GetPropagation();
            currentAmplitude = photon.GetAmplitude();

            float tilesPerSecond = PhotonMovementManager.MoveSpeed * PhotonMovementManager.ClassicSpeedMultiplier;
            timeToTravelTile = 1f / tilesPerSecond;
            waitForEndOfFrame = new();
        }

        public override void SetSource(Photon photon)
        {
            CreateNewDrawSprite(photon);
            base.SetSource(photon);
        }

        #region Visuals syncing
        public override void SyncVisuals()
        {
            SyncDrawSprite();
            SyncColor();
            SyncShader();
        }
        private void SyncDrawSprite()
        {
            if (currentDrawSprite == null ||
                currentOrientation != source.GetPropagation() ||
                currentAmplitude != source.GetAmplitude())
            {
                CreateNewDrawSprite();
                SyncPosition();
            }
        }
        private void SyncColor()
        {
            currentDrawSprite.color = source.GetColor();
        }
        private void SyncShader()
        {
            Vector2Int sourceDirection = source.GetPropagationIntVector();
            currentDrawSprite.material.SetFloat("_DirectionX", sourceDirection.x);
            currentDrawSprite.material.SetFloat("_DirectionY", sourceDirection.y);
        }
        private void SyncPosition()
        {
            Vector2 newPosition = GridUtils.GridPos2WorldPos(source.GetPosition(), openGrid);
            currentDrawSprite.transform.position = newPosition;
        }
        #endregion

        private void CreateNewDrawSprite(Photon externalSource = null, float? customAngle = null)
        {
            Photon photonSource = externalSource == null ? source : externalSource;
            currentDrawSprite = Instantiate(sprite, visualsHolder);
            currentOrientation = photonSource.GetPropagation();
            currentAmplitude = photonSource.GetAmplitude();

            Vector2 lookDir = currentOrientation.ToVector2();
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg;
            if(customAngle.HasValue) angle = customAngle.Value;
            currentDrawSprite.transform.eulerAngles = new Vector3(0, 0, angle);
        }

        #region Handle enter, exit and destroy
        protected override void HandleEnterComponent(OpticComponent component)
        {
            base.HandleEnterComponent(component);
        }
        protected override void HandleExitComponent(OpticComponent component)
        {
            if(isInComponent) SyncVisuals();
            base.HandleExitComponent(component);
        }
        protected override void HandleDestroySource(bool destroyVisuals)
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            if (destroyVisuals) Destroy(gameObject);
        }
        #endregion

        #region Handle movement
        public override void StartMovement() 
        {
            moveRoutine = StartCoroutine(MoveCo());
        }

        public void ForceMoveAlongNodes(Vector2[] nodes, ComponentPort outPort = null)
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            moveRoutine = StartCoroutine(ForceMoveAlongNodesCo(nodes, outPort));
        }

        private IEnumerator ForceMoveAlongNodesCo(Vector2[] nodes, ComponentPort outPort = null)
        {
            List<Vector2> nodeList = nodes.ToList();
            if (outPort != null) nodeList.Add(outPort.position);

            for (int i = 0; i < nodeList.Count; i++)
            {
            }
            yield break;
        }

        private IEnumerator MoveCo()
        {
            Vector2 moveStep = (source.GetPropagationIntVector() * openGrid.spacing);
            float stretchScale = currentDrawSprite.transform.localScale.x - sprite.transform.localScale.x;

            while (true)
            {
                Vector2 startPos = currentDrawSprite.transform.position;
                Vector2 endPos = startPos + (moveStep / 2);
                stretchScale += moveStep.magnitude;
                
                currentDrawSprite.transform.position = endPos;
                Vector3 scale = currentDrawSprite.transform.localScale;
                currentDrawSprite.transform.localScale = new Vector3(stretchScale, scale.y, scale.z);

                yield return new WaitForSeconds(timeToTravelTile);
            }
        }

        #endregion
    }
}
