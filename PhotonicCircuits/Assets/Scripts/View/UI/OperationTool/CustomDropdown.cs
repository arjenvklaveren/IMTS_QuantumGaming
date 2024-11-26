using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomDropdown : MonoBehaviour
{
    [SerializeField] Button mainButton;
    [SerializeField] GameObject contentBox;
    [SerializeField] RectTransform dropdownArrow;

    bool isOpen;

    private void Start()
    {
        Open();
        mainButton.onClick.AddListener(Toggle);
        Close();
    }

    public void UpdateText(TextMeshProUGUI panelText)
    {
        mainButton.GetComponentInChildren<TextMeshProUGUI>().text = panelText.text;
    }

    public void Toggle()
    {
        if (isOpen) Close();
        else Open();
    }
    public void Open()
    {
        dropdownArrow.rotation = Quaternion.Euler(0, 0, 180);
        contentBox.SetActive(true);
        isOpen = true;
    }
    public void Close()
    {
        dropdownArrow.rotation = Quaternion.Euler(0, 0, 0);
        contentBox.SetActive(false);
        isOpen = false;
    }
}
