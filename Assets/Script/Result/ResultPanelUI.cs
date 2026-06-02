using UnityEngine;
using UnityEngine.UI;

public class ResultPanelUI : MonoBehaviour
{
    [Header("루트")]
    [SerializeField] private GameObject resultRoot;

    [Header("텍스트")]
    [SerializeField] private Text dayText;
    [SerializeField] private Text expText;
    [SerializeField] private Text satisfactionText;
    [SerializeField] private Text influenceText;
    [SerializeField] private Text goldText;

    [Header("연결")]
    [SerializeField] private GameStatusUI gameStatusUI;

    private void Start()
    {
        // 씬 복귀 시 GameFlowController가 먼저 Result 상태로 복원한 경우,
        // 여기서 무조건 꺼버리면 결과창이 안 보이는 문제가 생긴다.
        if (GameFlowController.Instance != null &&
            GameFlowController.Instance.currentState == GameFlowState.Result)
        {
            OpenResult();
            return;
        }

        if (resultRoot != null)
            resultRoot.SetActive(false);
    }

    public void OpenResult()
    {
        if (DailyResultManager.Instance == null)
        {
            Debug.LogWarning("[ResultPanelUI] DailyResultManager.Instance가 없습니다.");
            return;
        }

        if (gameStatusUI != null && dayText != null)
            dayText.text = $"{GetCurrentDayText()}";

        if (expText != null)
            expText.text = $"경험치 획득 : {DailyResultManager.Instance.PendingExp}";

        if (satisfactionText != null)
            satisfactionText.text = $"만족도 획득 : {DailyResultManager.Instance.PendingSatisfaction}";

        if (influenceText != null)
            influenceText.text = $"누적영향력 획득 : {DailyResultManager.Instance.PendingInfluence}";

        if (goldText != null)
            goldText.text = $"수익 : {DailyResultManager.Instance.PendingGold}";

        if (resultRoot != null)
            resultRoot.SetActive(true);
    }

    public void ConfirmAndNextDay()
    {
        CloseResult();

        if (AugmentManager.Instance != null)
        {
            AugmentManager.Instance.OpenRandomAugments();
            Debug.Log("[ResultPanelUI] 결과 확인 후 증강 선택창을 엽니다.");
            return;
        }

        Debug.LogWarning("[ResultPanelUI] AugmentManager.Instance가 없습니다. 기존 방식으로 다음 날 처리합니다.");

        if (GameFlowController.Instance != null)
        {
            GameFlowController.Instance.OnResultConfirmed();
            return;
        }

        Debug.LogWarning("[ResultPanelUI] GameFlowController.Instance가 없습니다.");
    }

    public void CloseResult()
    {
        if (resultRoot != null)
            resultRoot.SetActive(false);
    }

    private string GetCurrentDayText()
    {
        if (gameStatusUI == null) return "n일째";
        return $"{gameStatusUI.CurrentDay}일째";
    }
}