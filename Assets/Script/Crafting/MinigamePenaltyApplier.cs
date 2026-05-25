using UnityEngine;

public class MinigamePenaltyApplier : MonoBehaviour
{
    [SerializeField] private GameStatusUI gameStatusUI;

    private void Start()
    {
        int penalty = MinigamePenaltyData.ConsumePenalty();

        if (penalty <= 0)
            return;

        if (gameStatusUI == null)
        {
            Debug.LogWarning("[MinigamePenaltyApplier] GameStatusUI가 연결되지 않았습니다.");
            return;
        }

        gameStatusUI.ReduceSatisfaction(penalty);
        Debug.Log($"[MinigamePenaltyApplier] 미니게임 실패 패널티 적용: 만족도 -{penalty}");
    }
}