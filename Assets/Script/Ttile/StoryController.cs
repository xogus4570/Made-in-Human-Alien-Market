using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoryController : MonoBehaviour
{
    [Header("�г�")]
    [SerializeField] private GameObject titleMenuPanel;
    [SerializeField] private GameObject storyPanel;

    [Header("��ư")]
    [SerializeField] private GameObject nextButton;

    [Header("���丮 �̹�����")]
    [SerializeField] private GameObject[] storyImages;

    [Header("�Ϲ� ��� UI")]
    [SerializeField] private TextMeshProUGUI storyText;

    [Header("��Ʈ�� ���� ȭ�� UI")]
    [SerializeField] private Image blackOverlay;
    [SerializeField] private TextMeshProUGUI introText;

    [Header("���丮 �ؽ�Ʈ")]
    [TextArea(2, 5)]
    [SerializeField] private string[] storyTexts;

    [Header("��Ʈ�� �ؽ�Ʈ")]
    [TextArea(2, 5)]
    [SerializeField] private string introStoryText;

    [Header("���� �ӵ�")]
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private float fadeSpeed = 0.7f;

    private int currentIndex = 0;
    private bool isTyping = false;

    private void Start()
    {
        if (storyPanel != null)
            storyPanel.SetActive(false);

        HideAllImages();

        if (blackOverlay != null)
            blackOverlay.gameObject.SetActive(false);

        if (introText != null)
        {
            introText.text = "";
            introText.gameObject.SetActive(false);
        }

        if (storyText != null)
            storyText.text = "";

        if (nextButton != null)
            nextButton.SetActive(false);
    }

    public void StartStory()
    {
        if (titleMenuPanel != null)
            titleMenuPanel.SetActive(false);

        if (storyPanel != null)
            storyPanel.SetActive(true);

        if (nextButton != null)
            nextButton.SetActive(false);

        currentIndex = 0;
        StartCoroutine(PlayIntroStory());
    }

    public void NextStory()
    {
        if (isTyping) return;

        currentIndex++;

        if (currentIndex >= storyImages.Length)
        {
            Debug.Log("[Story] ���丮 ����. ���߿� ���� ���� �� �̵� ����");
            return;
        }

        ShowStory(currentIndex);
    }

    private IEnumerator PlayIntroStory()
    {
        isTyping = true;

        HideAllImages();

        if (storyText != null)
            storyText.text = "";

        if (blackOverlay != null)
        {
            blackOverlay.gameObject.SetActive(true);

            Color c = blackOverlay.color;
            c.a = 1f;
            blackOverlay.color = c;
        }

        if (introText != null)
        {
            introText.gameObject.SetActive(true);
            introText.text = "";

            foreach (char ch in introStoryText)
            {
                introText.text += ch;
                yield return new WaitForSeconds(typingSpeed);
            }
        }

        yield return new WaitForSeconds(0.8f);

        if (storyImages.Length > 0 && storyImages[0] != null)
            storyImages[0].SetActive(true);

        if (nextButton != null)
            nextButton.SetActive(true);

        if (blackOverlay != null)
        {
            Color c = blackOverlay.color;

            while (c.a > 0f)
            {
                c.a -= Time.deltaTime * fadeSpeed;
                blackOverlay.color = c;
                yield return null;
            }

            blackOverlay.gameObject.SetActive(false);
        }

        if (introText != null)
            introText.gameObject.SetActive(false);

        if (storyTexts.Length > 0)
            yield return StartCoroutine(TypeStoryText(storyTexts[0]));
        else
            isTyping = false;
    }

    private void ShowStory(int index)
    {
        StopAllCoroutines();

        HideAllImages();

        if (storyImages[index] != null)
            storyImages[index].SetActive(true);

        if (index < storyTexts.Length)
            StartCoroutine(TypeStoryText(storyTexts[index]));
        else
            storyText.text = "";
    }

    private IEnumerator TypeStoryText(string text)
    {
        isTyping = true;

        if (storyText != null)
            storyText.text = "";

        foreach (char ch in text)
        {
            if (storyText != null)
                storyText.text += ch;

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void HideAllImages()
    {
        foreach (GameObject image in storyImages)
        {
            if (image != null)
                image.SetActive(false);
        }
    }
}