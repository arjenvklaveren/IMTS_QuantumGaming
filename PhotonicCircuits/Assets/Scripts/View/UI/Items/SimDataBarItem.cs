using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Game.UI
{
    public class SimDataBarItem : MonoBehaviour
    {
        [SerializeField] RectTransform holderRect;
        [SerializeField] RectTransform barRect;
        [SerializeField] TextMeshProUGUI valueText;
        [SerializeField] TextMeshProUGUI referenceText;

        float barValue = 0;
        int triggerCount = 0;

        public void SetBarValue01(float value)
        {
            value = Mathf.Clamp01(value);
            barValue = value;
            UpdateBarHeightVisual();
        }

        public void SetReferenceText(string text)
        {
            referenceText.text = text;  
        }

        public void AddCount() { triggerCount++; }
        public int GetCount() { return triggerCount; }
        public void ResetCount() { triggerCount = 0; }  

        void UpdateBarHeightVisual()
        {
            barRect.DOKill();
            float targetBarHeight = holderRect.rect.height * barValue;
            float currentBarHeight = barRect.sizeDelta.y;

            DOTween.To(() => currentBarHeight, x =>
            {
                currentBarHeight = x;
                barRect.sizeDelta = new Vector2(barRect.sizeDelta.x, currentBarHeight);
            }, targetBarHeight, 0.25f).SetEase(Ease.OutCubic);

            valueText.text = System.Math.Round(barValue, 2).ToString();
        }
    }
}
