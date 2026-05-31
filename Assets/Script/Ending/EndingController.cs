using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingController : MonoBehaviour
{
    [System.Serializable]
    public class EndingData
    {
        public EndingType endingType;
        public GameObject endingPanel;
        public GameObject[] endingImages;

        [TextArea(2, 5)]
        public string[] endingTexts;
    }

    [Header("공용 UI")]
    [SerializeField] private GameObject dialogueBackground;
    [SerializeField] private TextMeshProUGUI endingText;
    [SerializeField] private GameObject nextButton;

    [Header("엔딩 데이터")]
    [SerializeField] private EndingData[] endings;

    [Header("씬 이동")]
    [SerializeField] private string titleSceneName = "TitleScene";

    private EndingData currentEnding;
    private int currentIndex = 0;

    private void Start()
    {
        HideAllEndingPanels();

        if (dialogueBackground != null)
            dialogueBackground.SetActive(false);

        if (endingText != null)
            endingText.text = "";

        if (nextButton != null)
            nextButton.SetActive(false);

        EndingType savedEndingType =
            (EndingType)PlayerPrefs.GetInt("EndingType", (int)EndingType.BadEnding2);

        PlayEnding(savedEndingType);
    }

    public void PlayEnding(EndingType endingType)
    {
        HideAllEndingPanels();

        currentEnding = FindEndingData(endingType);
        currentIndex = 0;

        if (currentEnding == null)
        {
            Debug.LogError($"[Ending] {endingType} 데이터가 없습니다.");
            return;
        }

        if (currentEnding.endingPanel != null)
            currentEnding.endingPanel.SetActive(true);

        if (dialogueBackground != null)
            dialogueBackground.SetActive(true);

        if (nextButton != null)
            nextButton.SetActive(true);

        ShowCurrentEndingPage();
    }

    public void NextEnding()
    {
        if (currentEnding == null)
            return;

        currentIndex++;

        if (currentIndex >= currentEnding.endingImages.Length)
        {
            Debug.Log("[Ending] 엔딩 종료. 타이틀로 이동");
            SceneManager.LoadScene(titleSceneName);
            return;
        }

        ShowCurrentEndingPage();
    }

    private void ShowCurrentEndingPage()
    {
        HideCurrentEndingImages();

        if (currentIndex < currentEnding.endingImages.Length &&
            currentEnding.endingImages[currentIndex] != null)
        {
            currentEnding.endingImages[currentIndex].SetActive(true);
        }

        if (endingText != null)
        {
            if (currentIndex < currentEnding.endingTexts.Length)
                endingText.text = currentEnding.endingTexts[currentIndex];
            else
                endingText.text = "";
        }
    }

    private EndingData FindEndingData(EndingType endingType)
    {
        foreach (EndingData data in endings)
        {
            if (data != null && data.endingType == endingType)
                return data;
        }

        return null;
    }

    private void HideAllEndingPanels()
    {
        if (endings == null) return;

        foreach (EndingData data in endings)
        {
            if (data != null && data.endingPanel != null)
                data.endingPanel.SetActive(false);
        }
    }

    private void HideCurrentEndingImages()
    {
        if (currentEnding == null || currentEnding.endingImages == null)
            return;

        foreach (GameObject image in currentEnding.endingImages)
        {
            if (image != null)
                image.SetActive(false);
        }
    }
}