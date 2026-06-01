using System.Collections;
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

    [Header("타이핑 설정")]
    [SerializeField] private float typingSpeed = 0.04f;

    private Coroutine typingCoroutine;
    private bool isTyping;

    private void Start()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);

        if (dialogText != null)
            dialogText.text = "";
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

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeDialog(message));
    }

    private IEnumerator TypeDialog(string message)
    {
        isTyping = true;

        if (dialogText != null)
            dialogText.text = "";

        foreach (char ch in message)
        {
            if (dialogText != null)
                dialogText.text += ch;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void Close()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        isTyping = false;

        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }
}