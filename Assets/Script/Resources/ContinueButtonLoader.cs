using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ContinueButtonLoader : MonoBehaviour
{
    [Header("버튼")]
    [SerializeField] private Button continueButton;

    [Header("씬 이름")]
    [SerializeField] private string mainSceneName = "GwonTaeHyeon_Test";

    private void Awake()
    {
        if (continueButton == null)
            continueButton = GetComponent<Button>();
    }

    private void Start()
    {
        RefreshButtonState();
    }

    public void RefreshButtonState()
    {
        bool hasSave = SaveGameManager.HasSave();

        if (continueButton != null)
            continueButton.interactable = hasSave;

        Debug.Log(hasSave
            ? "[이어하기] 저장 파일이 있어 이어하기 버튼을 활성화했습니다."
            : "[이어하기] 저장 파일이 없어 이어하기 버튼을 비활성화했습니다.");
    }

    public void ContinueGame()
    {
        if (!SaveGameManager.HasSave())
        {
            Debug.LogWarning("[이어하기] 저장 파일이 없습니다.");
            RefreshButtonState();
            return;
        }

        SaveGameManager.RequestLoadOnNextMainScene();

        Debug.Log("[이어하기] 메인 씬으로 이동합니다.");
        SceneManager.LoadScene(mainSceneName, LoadSceneMode.Single);
    }
}