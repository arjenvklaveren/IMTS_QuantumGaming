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
            transform.position = GridUtils.GridPos2WorldPos(source.GetPosition(), openGrid);
            currentOrientation = photon.GetPropagation();
            currentAmplitude = photon.GetAmplitude();
        }

        public void ChangeSource(Photon photon)
        {
            source = photon;
            SetupListeners();
            CreateNewDrawSprite();
            SyncPosition();
            SyncVisuals();
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

        private void CreateNewDrawSprite()
        {
            currentDrawSprite = Instantiate(sprite, visualsHolder);
            currentOrientation = source.GetPropagation();
            currentAmplitude = source.GetAmplitude();
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
        protected override void HandleDestroySource()
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);

            //Destroy(gameObject);
        }
        #endregion

        #region Handle movement
        public override void StartMovement() 
        {
            moveRoutine = StartCoroutine(MoveCo());
        }
        private IEnumerator MoveCo()
        {
            Vector2 moveStep = (source.GetPropagationIntVector() * openGrid.spacing) / 2;
            Vector2 stretchScale = currentDrawSprite.transform.localScale;
            Vector2 strechStep = moveStep * 2;

            while (true)
            {
                Vector2 startPos = currentDrawSprite.transform.position;
                Vector2 endPos = startPos + moveStep;
                stretchScale += strechStep;

                currentDrawSprite.transform.position = endPos;
                currentDrawSprite.transform.localScale = stretchScale;
                yield return new WaitForSeconds(timeToTravelTile);
            }
        }
        #endregion
    }
}
