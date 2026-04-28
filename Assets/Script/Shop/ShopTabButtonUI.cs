using UnityEngine;
using UnityEngine.UI;

public class ShopTabButtonUI : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Text labelText;

    [SerializeField] private Color normalBackgroundColor = Color.white;
    [SerializeField] private Color selectedBackgroundColor = Color.gray;

    [SerializeField] private Color normalTextColor = Color.black;
    [SerializeField] private Color selectedTextColor = Color.black;

    public void SetSelected(bool selected)
    {
        if (backgroundImage != null)
            backgroundImage.color = selected ? selectedBackgroundColor : normalBackgroundColor;

        if (labelText != null)
            labelText.color = selected ? selectedTextColor : normalTextColor;
    }
}