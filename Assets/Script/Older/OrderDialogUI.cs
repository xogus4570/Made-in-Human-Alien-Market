using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderDialogUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialogPanel;

    [SerializeField] private Image customerImage;
    [SerializeField] private TextMeshProUGUI customerNameText;
    [SerializeField] private TextMeshProUGUI dialogText;

    private void Start()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    public void Open(string customerName, Sprite customerSprite, string message)
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(true);

        if (customerImage != null)
        {
            customerImage.sprite = customerSprite;
            customerImage.enabled = customerSprite != null;
        }

        if (customerNameText != null)
            customerNameText.text = customerName;

        if (dialogText != null)
            dialogText.text = message;
    }

    public void Close()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }
}