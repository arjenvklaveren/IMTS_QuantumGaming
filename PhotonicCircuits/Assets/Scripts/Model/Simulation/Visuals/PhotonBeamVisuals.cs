using Game.Data;
using System.Collections;
using System.Collections.Generic;
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

        private void CreateNewDrawSprite(Photon externalSource = null)
        {
            Photon photonSource = externalSource == null ? source : externalSource;
            currentDrawSprite = Instantiate(sprite, visualsHolder);
            currentOrientation = photonSource.GetPropagation();
            currentAmplitude = photonSource.GetAmplitude();
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
        private IEnumerator MoveCo()
        {
            Vector2 moveStep = (source.GetPropagationIntVector() * openGrid.spacing);
            Vector2 stretchScale = currentDrawSprite.transform.localScale -= sprite.transform.localScale;
            if(moveStep.x != 0) stretchScale.y = sprite.transform.localScale.y;
            if(moveStep.y != 0) stretchScale.x = sprite.transform.localScale.x;

            while (true)
            {
                Vector2 startPos = currentDrawSprite.transform.position;
                Vector2 endPos = startPos + (moveStep / 2);
                stretchScale += moveStep;

                currentDrawSprite.transform.position = endPos;
                currentDrawSprite.transform.localScale = stretchScale;
                //yield break;
                yield return new WaitForSeconds(timeToTravelTile);
            }
        }
        #endregion
    }
}
