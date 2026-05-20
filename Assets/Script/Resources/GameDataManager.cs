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

    [Header("시간 데이터")]
    public int currentHour = 9;
    public int currentMinute = 0;
    public float timeTickTimer = 0f;
    public float lastRealtime = 0f;

    [Header("FSM 상태 데이터")]
    public GameFlowState flowState = GameFlowState.Ready;

    [Header("위치 데이터")]
    public bool hasPlayerPosition = false;
    public Vector3 playerPosition;

    public bool hasInspectorPosition = false;
    public Vector3 inspectorPosition;
    public bool inspectorWasActive = false;

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