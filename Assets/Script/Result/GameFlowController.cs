using UnityEngine;
using UnityEngine.UI;

public class GameFlowController : MonoBehaviour
{
    public static GameFlowController Instance;

    [Header("현재 상태")]
    public GameFlowState currentState = GameFlowState.Ready;

    [Header("기본 연결")]
    [SerializeField] private GameStatusUI gameStatusUI;
    [SerializeField] private ResultPanelUI resultPanelUI;

    [Header("영업 제어")]
    [SerializeField] private PhoneOrderAutoSpawner phoneOrderAutoSpawner;

    [Header("상점 제어")]
    [SerializeField] private GameObject shopObject;

    [Header("시작 버튼 제어")]
    [SerializeField] private GameObject startObject;

    [Header("시간 UI")]
    [SerializeField] private Text timeText;

    [Header("시간 설정")]
    [SerializeField] private int startHour = 9;
    [SerializeField] private int startMinute = 0;
    [SerializeField] private int endHour = 12;
    [SerializeField] private int endMinute = 0;

    [Header("시간 흐름 설정")]
    [SerializeField] private float timeTickSec = 5f;
    [SerializeField] private int gameMinutePerTick = 15;

    private bool isTimeRunning;

    private int CurrentHour
    {
        get
        {
            if (GameDataManager.Instance != null)
                return GameDataManager.Instance.currentHour;

            return startHour;
        }
        set
        {
            if (GameDataManager.Instance != null)
                GameDataManager.Instance.currentHour = value;
        }
    }

    private int CurrentMinute
    {
        get
        {
            if (GameDataManager.Instance != null)
                return GameDataManager.Instance.currentMinute;

            return startMinute;
        }
        set
        {
            if (GameDataManager.Instance != null)
                GameDataManager.Instance.currentMinute = value;
        }
    }

    private float TimeTickTimer
    {
        get
        {
            if (GameDataManager.Instance != null)
                return GameDataManager.Instance.timeTickTimer;

            return 0f;
        }
        set
        {
            if (GameDataManager.Instance != null)
                GameDataManager.Instance.timeTickTimer = value;
        }
    }

    private float LastRealtime
    {
        get
        {
            if (GameDataManager.Instance != null)
                return GameDataManager.Instance.lastRealtime;

            return Time.realtimeSinceStartup;
        }
        set
        {
            if (GameDataManager.Instance != null)
                GameDataManager.Instance.lastRealtime = value;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (GameDataManager.Instance != null)
        {
            currentState = GameDataManager.Instance.flowState;
            ApplyStateWithoutTransition();
        }
        else
        {
            EnterReady();
        }
    }

    private void Update()
    {
        UpdateBusinessTime();

        // Result 전환은 아직 테스트용 R키/숫자 3키 유지
        if (currentState == GameFlowState.Play &&
            (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Alpha3)))
        {
            EnterResult();
        }
    }

    public void OnClickStartPlay()
    {
        if (currentState != GameFlowState.Ready)
        {
            Debug.LogWarning("[FSM] Ready 상태가 아니라 영업 시작 버튼 입력을 무시합니다.");
            return;
        }

        EnterPlay();
    }

    public void EnterReady()
    {
        currentState = GameFlowState.Ready;

        if (GameDataManager.Instance != null)
            GameDataManager.Instance.flowState = GameFlowState.Ready;

        isTimeRunning = false;
        ResetBusinessTime();

        if (phoneOrderAutoSpawner != null)
            phoneOrderAutoSpawner.enabled = false;

        if (shopObject != null)
            shopObject.SetActive(true);

        if (startObject != null)
            startObject.SetActive(true);

        Debug.Log("[FSM] Ready 상태: 준비 단계 / 상점 이용 가능 / 영업 시작 버튼 ON / 주문 생성 OFF / 시간 09:00");
    }

    public void EnterPlay()
    {
        currentState = GameFlowState.Play;

        if (GameDataManager.Instance != null)
            GameDataManager.Instance.flowState = GameFlowState.Play;

        ResetBusinessTime();
        LastRealtime = Time.realtimeSinceStartup;
        isTimeRunning = true;

        if (phoneOrderAutoSpawner != null)
            phoneOrderAutoSpawner.enabled = true;

        if (shopObject != null)
            shopObject.SetActive(false);

        if (startObject != null)
            startObject.SetActive(false);

        Debug.Log("[FSM] Play 상태: 영업 시작 / 상점 이용 불가 / 영업 시작 버튼 OFF / 주문 생성 ON / 시간 흐름 시작");
    }

    public void EnterResult()
    {
        currentState = GameFlowState.Result;

        if (GameDataManager.Instance != null)
            GameDataManager.Instance.flowState = GameFlowState.Result;

        isTimeRunning = false;
        UpdateTimeUI();

        if (phoneOrderAutoSpawner != null)
            phoneOrderAutoSpawner.enabled = false;

        if (shopObject != null)
            shopObject.SetActive(false);

        if (startObject != null)
            startObject.SetActive(false);

        Debug.Log("[FSM] Result 상태: 결과창 표시 / 상점 이용 불가 / 영업 시작 버튼 OFF / 주문 생성 OFF / 시간 정지");

        if (resultPanelUI != null)
            resultPanelUI.OpenResult();
    }

    public void OnResultConfirmed()
    {
        if (currentState != GameFlowState.Result)
        {
            Debug.LogWarning("[FSM] Result 상태가 아니라 확인 처리를 무시합니다.");
            return;
        }

        currentState = GameFlowState.NextDay;

        if (GameDataManager.Instance != null)
            GameDataManager.Instance.flowState = GameFlowState.NextDay;

        Debug.Log("[FSM] NextDay 상태: 다음 날 준비 처리");

        ProcessNextDay();
    }

    private void ProcessNextDay()
    {
        if (DailyResultManager.Instance != null && gameStatusUI != null)
        {
            DailyResultManager.Instance.ApplyTo(gameStatusUI);
            DailyResultManager.Instance.ClearDailyResult();
        }

        if (OrderManager.Instance != null)
            OrderManager.Instance.ClearAllOrders();

        if (gameStatusUI != null)
            gameStatusUI.NextDay();

        if (resultPanelUI != null)
            resultPanelUI.CloseResult();

        EnterReady();

        Debug.Log("[FSM] NextDay 완료 → Ready 복귀");
    }

    private void UpdateBusinessTime()
    {
        if (!isTimeRunning) return;
        if (currentState != GameFlowState.Play) return;

        float now = Time.realtimeSinceStartup;
        float elapsed = now - LastRealtime;
        LastRealtime = now;

        if (elapsed <= 0f) return;

        TimeTickTimer += elapsed;

        while (TimeTickTimer >= timeTickSec)
        {
            TimeTickTimer -= timeTickSec;

            AddGameMinutes(gameMinutePerTick);
            UpdateTimeUI();

            if (IsBusinessTimeEnded())
            {
                Debug.Log("[FSM] 영업 시간이 종료되어 Result 상태로 전환합니다.");
                EnterResult();
                break;
            }
        }
    }

    private void ApplyElapsedTimeAfterSceneReturn()
    {
        float now = Time.realtimeSinceStartup;

        if (LastRealtime <= 0f)
        {
            LastRealtime = now;
            UpdateTimeUI();
            return;
        }

        float elapsed = now - LastRealtime;
        LastRealtime = now;

        if (elapsed <= 0f)
        {
            UpdateTimeUI();
            return;
        }

        TimeTickTimer += elapsed;

        while (TimeTickTimer >= timeTickSec)
        {
            TimeTickTimer -= timeTickSec;

            AddGameMinutes(gameMinutePerTick);

            if (IsBusinessTimeEnded())
            {
                UpdateTimeUI();
                Debug.Log("[FSM] 씬 복귀 중 영업 시간이 종료되어 Result 상태로 전환합니다.");
                EnterResult();
                return;
            }
        }

        UpdateTimeUI();
    }

    private void ResetBusinessTime()
    {
        CurrentHour = startHour;
        CurrentMinute = startMinute;
        TimeTickTimer = 0f;
        LastRealtime = Time.realtimeSinceStartup;
        UpdateTimeUI();
    }

    private void AddGameMinutes(int minutes)
    {
        int hour = CurrentHour;
        int minute = CurrentMinute;

        minute += minutes;

        while (minute >= 60)
        {
            minute -= 60;
            hour++;
        }

        CurrentHour = hour;
        CurrentMinute = minute;
    }

    private bool IsBusinessTimeEnded()
    {
        if (CurrentHour > endHour) return true;
        if (CurrentHour == endHour && CurrentMinute >= endMinute) return true;
        return false;
    }

    private void UpdateTimeUI()
    {
        if (timeText != null)
            timeText.text = $"{CurrentHour:00}:{CurrentMinute:00}";
    }

    private void ApplyStateWithoutTransition()
    {
        if (currentState == GameFlowState.Ready)
        {
            isTimeRunning = false;
            ResetBusinessTime();

            if (phoneOrderAutoSpawner != null)
                phoneOrderAutoSpawner.enabled = false;

            if (shopObject != null)
                shopObject.SetActive(true);

            if (startObject != null)
                startObject.SetActive(true);

            Debug.Log("[FSM] Ready 상태 복원");
        }
        else if (currentState == GameFlowState.Play)
        {
            isTimeRunning = true;
            ApplyElapsedTimeAfterSceneReturn();

            if (currentState != GameFlowState.Play)
                return;

            if (phoneOrderAutoSpawner != null)
                phoneOrderAutoSpawner.enabled = true;

            if (shopObject != null)
                shopObject.SetActive(false);

            if (startObject != null)
                startObject.SetActive(false);

            Debug.Log("[FSM] Play 상태 복원");
        }
        else if (currentState == GameFlowState.Result)
        {
            isTimeRunning = false;
            UpdateTimeUI();

            if (phoneOrderAutoSpawner != null)
                phoneOrderAutoSpawner.enabled = false;

            if (shopObject != null)
                shopObject.SetActive(false);

            if (startObject != null)
                startObject.SetActive(false);

            if (resultPanelUI != null)
                resultPanelUI.OpenResult();

            Debug.Log("[FSM] Result 상태 복원");
        }
        else if (currentState == GameFlowState.NextDay)
        {
            EnterReady();
        }
    }
}