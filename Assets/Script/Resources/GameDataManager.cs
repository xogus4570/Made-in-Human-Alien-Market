using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("게임 진행 데이터")]
    public int day = 1;
    public int gold = 9999;
    public int satisfaction = 100;
    public int influence = 9999;
    public int level = 1;
    public int currentExp = 0;

    [Header("FSM 상태 데이터")]
    public GameFlowState flowState = GameFlowState.Ready;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[GameDataManager] KEEP");
        }
        else if (Instance != this)
        {
            Debug.Log("[GameDataManager] DESTROY DUP");
            Destroy(gameObject);
        }
    }
}