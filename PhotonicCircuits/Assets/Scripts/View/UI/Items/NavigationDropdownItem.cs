using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Game.UI
{
    public class NavigationDropdownItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] List<string> placeholderItems = new List<string>();

        [Header("Object references")]
        [SerializeField] Button mainButton;
        [SerializeField] GameObject referenceButtonHolder;
        [SerializeField] GameObject contentHolder;

        #region Initialisation
        private void Start()
        {
            SyncPlaceholderItems();
            SetupListeners();
        }

        private void OnDestroy()
        {
            RemoveListeners();
        }

        void SetupListeners()
        {
            mainButton.onClick.AddListener(() => SetContentHolderState(true));
        }

        void RemoveListeners()
        {
            mainButton.onClick.RemoveListener(() => SetContentHolderState(true));
        }
        #endregion

        void SyncPlaceholderItems()
        {
            DestroyPlaceHolderItems();
            CreatePlaceHolderItems();
        }

        void CreatePlaceHolderItems()
        {
            referenceButtonHolder.SetActive(false);
            foreach (string placeholder in placeholderItems)
            {
                GameObject placeHolderItem = Instantiate(referenceButtonHolder, contentHolder.transform);
                placeHolderItem.GetComponentInChildren<TextMeshProUGUI>().text = placeholder;
                placeHolderItem.gameObject.SetActive(true);
            }
        }

        void DestroyPlaceHolderItems()
        {
            foreach(Transform child in contentHolder.transform)
            {
                if (child == referenceButtonHolder.transform) return;
                Destroy(child.gameObject);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //SetContentHolderState(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!eventData.fullyExited) return;
            SetContentHolderState(false);
            EventSystem.current.SetSelectedGameObject(null);
        }

        private void SetContentHolderState(bool state)
        {
            contentHolder.SetActive(state);
        }
    }
}
