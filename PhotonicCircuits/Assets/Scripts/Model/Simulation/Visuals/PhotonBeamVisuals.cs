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

            transform.position = GridUtils.GridPos2WorldPos(source.GetPosition(), openGrid);

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
            SyncShader(source);
        }
        private void SyncDrawSprite()
        {
            if (currentDrawSprite == null ||
                currentOrientation != source.GetPropagation() ||
                currentAmplitude != source.GetAmplitude() ||
                DrawSpriteIsStillInNodes()
                )
            {
                CreateNewDrawSprite();
                SyncPosition();
            }
        }
        private void SyncShader(Photon sourceP)
        {
            Vector2Int sourceDirection = sourceP.GetPropagationIntVector();
            float moveSpeed = PhotonMovementManager.MoveSpeed * PhotonMovementManager.ClassicSpeedMultiplier;

            currentDrawSprite.material.SetFloat("_LineMoveSpeed", moveSpeed);
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
            currentDrawSprite.color = photonSource.GetColor();
            currentOrientation = photonSource.GetPropagation();
            currentAmplitude = photonSource.GetAmplitude();
            SyncShader(photonSource);

            Vector2 lookDir = currentOrientation.ToVector2();
            SetCurrentDrawSpriteAngleByLookDir(lookDir);
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
        protected override void HandleDestroySource(bool storeVisuals)
        {
            if (moveRoutine != null)
                StopCoroutine(moveRoutine);            

            if(storeVisuals) PhotonManager.Instance.StoreBeamVisual(this);
            else Destroy(gameObject);
        }
        #endregion

        #region Handle movement
        public override void StartMovement() 
        {
            moveRoutine = StartCoroutine(MoveCo());
        }

        public override void ForceMoveAlongNodes(Vector2[] nodes, ComponentPort outPort = null)
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
                Vector2 currentTipPos = GetCurrentDrawSpriteTipPos();
                float dist = Vector2.Distance(currentTipPos, nodeList[i]);
                float duration = timeToTravelTile * dist;

                if (dist == 0) continue;

                if(currentDrawSprite.transform.localScale.x != 0) CreateNewDrawSprite();
                currentDrawSprite.transform.position = currentTipPos;
                SetCurrentDrawSpriteAngleByLookDir(nodeList[i] - currentTipPos);

                yield return StartCoroutine(MoveCurrentDrawSpriteCo(currentTipPos, nodeList[i], duration));
            }
        }

        private IEnumerator MoveCo()
        {
            Vector2 moveStep = (source.GetPropagationIntVector() * openGrid.spacing);

            while (true)
            {
                Vector2 startPos = currentDrawSprite.transform.position;
                yield return StartCoroutine(MoveCurrentDrawSpriteCo(startPos, startPos + moveStep, timeToTravelTile));
            }
        }

        private IEnumerator MoveCurrentDrawSpriteCo(Vector2 startPos, Vector2 endPos, float duration)
        {
            float startScale = currentDrawSprite.transform.localScale.x;
            float endScale = startScale + Vector2.Distance(startPos, endPos);

            endPos = startPos + ((endPos - startPos) / 2);

            float timer = duration;

            while (timer > 0f)
            {
                yield return waitForEndOfFrame;
                timer -= Time.deltaTime;

                currentDrawSprite.transform.position = Vector2.Lerp(startPos, endPos, 1f - (timer / duration));
                Vector3 scale = currentDrawSprite.transform.localScale;
                currentDrawSprite.transform.localScale = new Vector3(Mathf.Lerp(startScale, endScale, 1f - (timer / duration)), scale.y, scale.z);
            }
        }

        private void SetCurrentDrawSpriteAngleByLookDir(Vector2 lookDir)
        {
            float angle = GetAngleByVec2(lookDir);
            currentDrawSprite.transform.eulerAngles = new Vector3(0, 0, angle);
        }
        private bool DrawSpriteIsStillInNodes()
        {
            float drawSpriteAngle = GetAngleByVec2(currentDrawSprite.transform.right);
            return !(Mathf.Abs(drawSpriteAngle) == 0 || (Mathf.Abs(drawSpriteAngle) == 90));
        }
        private float GetAngleByVec2(Vector2 vec) { return Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg; }

        private Vector2 GetCurrentDrawSpriteTipPos()
        {
            return currentDrawSprite.transform.position + (currentDrawSprite.transform.right * (currentDrawSprite.transform.localScale.x / 2));
        }
        #endregion
    }
}
