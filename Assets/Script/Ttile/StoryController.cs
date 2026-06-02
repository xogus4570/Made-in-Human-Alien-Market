using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class StoryController : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject titleMenuPanel;
    [SerializeField] private GameObject storyPanel;

    [Header("버튼")]
    [SerializeField] private GameObject nextButton;

    [Header("스토리 이미지")]
    [SerializeField] private GameObject[] storyImages;

    [Header("일반 대사 UI")]
    [SerializeField] private TextMeshProUGUI storyText;

    [Header("인트로 검은 화면 UI")]
    [SerializeField] private Image blackOverlay;
    [SerializeField] private TextMeshProUGUI introText;

    [Header("스토리 텍스트")]
    [TextArea(2, 5)]
    [SerializeField] private string[] storyTexts;

    [Header("인트로 텍스트")]
    [TextArea(2, 5)]
    [SerializeField] private string introStoryText;

    [Header("진행 속도")]
    [SerializeField] private float typingSpeed = 0.04f;
    [SerializeField] private float fadeSpeed = 0.7f;

    [Header("씬 이동")]
    [SerializeField] private string mainSceneName = "GwonTaeHyeon_Test";

    [Header("스킵 설정")]
    [SerializeField] private bool enableSkipKey = true;
    [SerializeField] private KeyCode skipKey = KeyCode.Space;

    private int currentIndex = 0;
    private bool isTyping = false;
    private bool storyStarted = false;
    private bool isMovingScene = false;

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

    private void Update()
    {
        if (!enableSkipKey)
            return;

        if (!storyStarted)
            return;

        if (isMovingScene)
            return;

        if (Input.GetKeyDown(skipKey))
        {
            Debug.Log($"[Story] 스킵 키 입력: {skipKey}. 메인 씬으로 이동합니다.");
            MoveToMainScene();
        }
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
        storyStarted = true;
        isMovingScene = false;

        StartCoroutine(PlayIntroStory());
    }

    public void NextStory()
    {
        if (isMovingScene)
            return;

        if (isTyping)
            return;

        currentIndex++;

        if (currentIndex >= storyImages.Length)
        {
            MoveToMainScene();
            return;
        }

        ShowStory(currentIndex);
    }

    private void MoveToMainScene()
    {
        if (isMovingScene)
            return;

        isMovingScene = true;

        StopAllCoroutines();

        if (string.IsNullOrEmpty(mainSceneName))
        {
            Debug.LogWarning("[Story] mainSceneName이 비어 있어 메인 씬으로 이동할 수 없습니다.");
            isMovingScene = false;
            return;
        }

        Debug.Log($"[Story] 메인 씬으로 이동합니다: {mainSceneName}");
        SceneManager.LoadScene(mainSceneName);
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
        else if (storyText != null)
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