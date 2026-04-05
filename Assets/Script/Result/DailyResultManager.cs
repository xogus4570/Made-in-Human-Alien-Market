using UnityEngine;

public class DailyResultManager : MonoBehaviour
{
    public static DailyResultManager Instance;

    private int pendingGold;
    private int pendingExp;
    private int pendingInfluence;
    private int pendingSatisfaction;

    public int PendingGold => pendingGold;
    public int PendingExp => pendingExp;
    public int PendingInfluence => pendingInfluence;
    public int PendingSatisfaction => pendingSatisfaction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void AddReward(int gold, int exp, int influence, int satisfaction)
    {
        pendingGold += gold;
        pendingExp += exp;
        pendingInfluence += influence;
        pendingSatisfaction += satisfaction;

        Debug.Log($"[DailyResult] 누적됨 Gold:{pendingGold}, Exp:{pendingExp}, Influence:{pendingInfluence}, Satisfaction:{pendingSatisfaction}");
    }

    public void ApplyTo(GameStatusUI gameStatusUI)
    {
        if (gameStatusUI == null)
        {
            Debug.LogWarning("[DailyResult] GameStatusUI가 없습니다.");
            return;
        }

        if (pendingGold != 0) gameStatusUI.AddGold(pendingGold);
        if (pendingExp != 0) gameStatusUI.AddExp(pendingExp);
        if (pendingInfluence != 0) gameStatusUI.AddInfluence(pendingInfluence);

        if (pendingSatisfaction > 0)
            gameStatusUI.AddSatisfaction(pendingSatisfaction);
        else if (pendingSatisfaction < 0)
            gameStatusUI.ReduceSatisfaction(-pendingSatisfaction);
    }

    public void ClearDailyResult()
    {
        pendingGold = 0;
        pendingExp = 0;
        pendingInfluence = 0;
        pendingSatisfaction = 0;
    }
}
