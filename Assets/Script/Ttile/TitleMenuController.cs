using UnityEngine;

public class TitleMenuController : MonoBehaviour
{
    [Header("패널")]
    [SerializeField] private GameObject titleMenuPanel;

    [Header("스토리 컨트롤러")]
    [SerializeField] private StoryController storyController;

    // 게임 시작 버튼
    public void OnClickStart()
    {
        Debug.Log("[Title] 게임 시작");

        if (titleMenuPanel != null)
        {
            titleMenuPanel.SetActive(false);
        }

        if (storyController != null)
        {
            storyController.StartStory();
        }
    }

    // 이어하기 / 연결 버튼
    public void OnClickConnect()
    {
        Debug.Log("[Title] Connect 버튼 클릭");
    }

    // 게임 종료 버튼
    public void OnClickExit()
    {
        Debug.Log("[Title] 게임 종료");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}