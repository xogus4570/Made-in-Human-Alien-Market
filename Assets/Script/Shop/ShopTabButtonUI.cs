using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopTabButtonUI : MonoBehaviour
{
    [Header("대상 UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI labelText;

    [Header("기본 상태")]
    [SerializeField] private Color normalBackgroundColor = Color.white;
    [SerializeField] private Color selectedBackgroundColor = Color.gray;

    [SerializeField] private Color normalTextColor = Color.black;
    [SerializeField] private Color selectedTextColor = Color.black;

    [Header("초기 상태")]
    [SerializeField] private bool isSelectedOnStart = false;

    private void Awake()
    {
        ApplyVisual(isSelectedOnStart);
    }

    public void SetSelected(bool selected)
    {
        ApplyVisual(selected);
    }

    private void ApplyVisual(bool selected)
    {
        if (backgroundImage != null)
            backgroundImage.color = selected ? selectedBackgroundColor : normalBackgroundColor;

        if (labelText != null)
            labelText.color = selected ? selectedTextColor : normalTextColor;
    }
}