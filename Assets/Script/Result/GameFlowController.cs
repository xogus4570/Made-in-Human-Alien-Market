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
    [SerializeField] private PlayerAnimationController playerAnimationController;

    [Header("영업 제어")]
    [SerializeField] private PhoneOrderAutoSpawner phoneOrderAutoSpawner;

    [Header("상점 제어")]
    [SerializeField] private GameObject shopObject;

    [Header("시작 버튼 제어")]
    [SerializeField] private GameObject startObject;

    [Header("시간 UI")]
    [SerializeField] private Text timeText;

    [Header("낮/밤 오버레이")]
    [SerializeField] private Image dayNightOverlay;

    [Header("시간 설정")]
    [SerializeField] private int startHour = 9;
    [SerializeField] private int startMinute = 0;
    [SerializeField] private int endHour = 24;
    [SerializeField] private int endMinute = 0;

    [Header("시간 흐름 설정")]
    [SerializeField] private float timeTickSec = 5f;
    [SerializeField] private int gameMinutePerTick = 15;

    [Header("테스트 키 설정")]
    [SerializeField] private bool enableResultTestKey = false;

    [SerializeField] private bool enableSatisfactionRecoverTestKey = false;
    [SerializeField] private KeyCode recoverSatisfactionKey = KeyCode.Alpha8;
    [SerializeField] private int recoverSatisfactionAmount = 100;

    [SerializeField] private bool enableSatisfactionDecreaseTestKey = false;
    [SerializeField] private KeyCode decreaseSatisfactionKey = KeyCode.Alpha9;
    [SerializeField] private int decreaseSatisfactionAmount = 10;

    [Header("상태별 낮/밤 알파값")]
    [SerializeField] private float readyOverlayAlpha = 0.75f;
    [SerializeField] private float resultOverlayAlpha = 0.00f;

    [Header("Play 시간대별 낮/밤 알파값")]
    [SerializeField] private float alpha09 = 0.05f;
    [SerializeField] private float alpha10 = 0.05f;
    [SerializeField] private float alpha11 = 0.05f;
    [SerializeField] private float alpha12 = 0.00f;
    [SerializeField] private float alpha13 = 0.00f;
    [SerializeField] private float alpha14 = 0.05f;
    [SerializeField] private float alpha15 = 0.10f;
    [SerializeField] private float alpha16 = 0.20f;
    [SerializeField] private float alpha17 = 0.35f;
    [SerializeField] private float alpha18 = 0.50f;
    [SerializeField] private float alpha19 = 0.60f;
    [SerializeField] private float alpha20 = 0.70f;
    [SerializeField] private float alpha21 = 0.80f;
    [SerializeField] private float alpha22 = 0.85f;
    [SerializeField] private float alpha23 = 0.90f;
    [SerializeField] private float alpha24 = 0.95f;

    [Header("엔딩")]
    [SerializeField] private EndingDecisionManager endingDecisionManager;

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
        if (dayNightOverlay != null)
            dayNightOverlay.raycastTarget = false;

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

        if (enableResultTestKey &&
            currentState == GameFlowState.Play &&
            (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Alpha3)))
        {
            EnterResult();
        }

        if (enableSatisfactionRecoverTestKey &&
            Input.GetKeyDown(recoverSatisfactionKey))
        {
            RecoverSatisfactionForTest();
        }

        if (enableSatisfactionDecreaseTestKey &&
            Input.GetKeyDown(decreaseSatisfactionKey))
        {
            DecreaseSatisfactionForTest();
        }
    }

    private void RecoverSatisfactionForTest()
    {
        if (gameStatusUI == null)
        {
            Debug.LogWarning("[FSM-Test] GameStatusUI가 연결되지 않았습니다.");
            return;
        }

        gameStatusUI.AddSatisfaction(recoverSatisfactionAmount);
        Debug.Log($"[FSM-Test] 고객만족도를 {recoverSatisfactionAmount} 회복했습니다.");
    }

    private void DecreaseSatisfactionForTest()
    {
        if (gameStatusUI == null)
        {
            Debug.LogWarning("[FSM-Test] GameStatusUI가 연결되지 않았습니다.");
            return;
        }

        gameStatusUI.ReduceSatisfaction(decreaseSatisfactionAmount);
        Debug.Log($"[FSM-Test] 고객만족도를 {decreaseSatisfactionAmount} 감소시켰습니다.");
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
        SetDayNightOverlayAlpha(readyOverlayAlpha);

        if (phoneOrderAutoSpawner != null)
            phoneOrderAutoSpawner.enabled = false;

        if (shopObject != null)
            shopObject.SetActive(true);

        if (startObject != null)
            startObject.SetActive(true);

        if (playerAnimationController != null)
            playerAnimationController.SetMasked(false);

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
        UpdateDayNightByCurrentTime();

        if (phoneOrderAutoSpawner != null)
            phoneOrderAutoSpawner.enabled = true;

        if (shopObject != null)
            shopObject.SetActive(false);

        if (startObject != null)
            startObject.SetActive(false);

        if (playerAnimationController != null)
            playerAnimationController.SetMasked(true);

        Debug.Log("[FSM] Play 상태: 영업 시작 / 상점 이용 불가 / 영업 시작 버튼 OFF / 주문 생성 ON / 시간 흐름 시작");
    }

    public void EnterResult()
    {
        currentState = GameFlowState.Result;

        if (GameDataManager.Instance != null)
            GameDataManager.Instance.flowState = GameFlowState.Result;

        isTimeRunning = false;
        UpdateTimeUI();
        SetDayNightOverlayAlpha(resultOverlayAlpha);

        if (phoneOrderAutoSpawner != null)
            phoneOrderAutoSpawner.enabled = false;

        if (shopObject != null)
            shopObject.SetActive(false);

        if (startObject != null)
            startObject.SetActive(false);

        if (playerAnimationController != null)
            playerAnimationController.SetMasked(true);

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

        if (gameStatusUI != null && gameStatusUI.CurrentDay >= 30)
        {
            if (resultPanelUI != null)
                resultPanelUI.CloseResult();

            if (endingDecisionManager != null)
            {
                endingDecisionManager.CheckAndMoveToEnding(
                    gameStatusUI.CurrentGold,
                    gameStatusUI.CurrentInfluence
                );
            }
            else
            {
                Debug.LogError("[FSM] EndingDecisionManager가 연결되지 않았습니다.");
            }

            return;
        }

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
            UpdateDayNightByCurrentTime();

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
            UpdateDayNightByCurrentTime();
            return;
        }

        float elapsed = now - LastRealtime;
        LastRealtime = now;

        if (elapsed <= 0f)
        {
            UpdateTimeUI();
            UpdateDayNightByCurrentTime();
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
                UpdateDayNightByCurrentTime();
                Debug.Log("[FSM] 씬 복귀 중 영업 시간이 종료되어 Result 상태로 전환합니다.");
                EnterResult();
                return;
            }
        }

        UpdateTimeUI();
        UpdateDayNightByCurrentTime();
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
        {
            int displayHour = CurrentHour % 24;
            timeText.text = $"{displayHour:00}:{CurrentMinute:00}";
        }
    }

    private void UpdateDayNightByCurrentTime()
    {
        if (currentState != GameFlowState.Play)
            return;

        float timeValue = CurrentHour + (CurrentMinute / 60f);
        float alpha = GetOverlayAlphaByTime(timeValue);

        SetDayNightOverlayAlpha(alpha);
    }

    private float GetOverlayAlphaByTime(float timeValue)
    {
        if (timeValue < 10f)
            return Mathf.Lerp(alpha09, alpha10, Mathf.InverseLerp(9f, 10f, timeValue));

        if (timeValue < 11f)
            return Mathf.Lerp(alpha10, alpha11, Mathf.InverseLerp(10f, 11f, timeValue));

        if (timeValue < 12f)
            return Mathf.Lerp(alpha11, alpha12, Mathf.InverseLerp(11f, 12f, timeValue));

        if (timeValue < 13f)
            return Mathf.Lerp(alpha12, alpha13, Mathf.InverseLerp(12f, 13f, timeValue));

        if (timeValue < 14f)
            return Mathf.Lerp(alpha13, alpha14, Mathf.InverseLerp(13f, 14f, timeValue));

        if (timeValue < 15f)
            return Mathf.Lerp(alpha14, alpha15, Mathf.InverseLerp(14f, 15f, timeValue));

        if (timeValue < 16f)
            return Mathf.Lerp(alpha15, alpha16, Mathf.InverseLerp(15f, 16f, timeValue));

        if (timeValue < 17f)
            return Mathf.Lerp(alpha16, alpha17, Mathf.InverseLerp(16f, 17f, timeValue));

        if (timeValue < 18f)
            return Mathf.Lerp(alpha17, alpha18, Mathf.InverseLerp(17f, 18f, timeValue));

        if (timeValue < 19f)
            return Mathf.Lerp(alpha18, alpha19, Mathf.InverseLerp(18f, 19f, timeValue));

        if (timeValue < 20f)
            return Mathf.Lerp(alpha19, alpha20, Mathf.InverseLerp(19f, 20f, timeValue));

        if (timeValue < 21f)
            return Mathf.Lerp(alpha20, alpha21, Mathf.InverseLerp(20f, 21f, timeValue));

        if (timeValue < 22f)
            return Mathf.Lerp(alpha21, alpha22, Mathf.InverseLerp(21f, 22f, timeValue));

        if (timeValue < 23f)
            return Mathf.Lerp(alpha22, alpha23, Mathf.InverseLerp(22f, 23f, timeValue));

        return Mathf.Lerp(alpha23, alpha24, Mathf.InverseLerp(23f, 24f, timeValue));
    }

    private void SetDayNightOverlayAlpha(float alpha)
    {
        if (dayNightOverlay == null)
            return;

        Color color = dayNightOverlay.color;
        color.a = Mathf.Clamp01(alpha);
        dayNightOverlay.color = color;
    }

    private void ApplyStateWithoutTransition()
    {
        if (currentState == GameFlowState.Ready)
        {
            isTimeRunning = false;
            ResetBusinessTime();
            SetDayNightOverlayAlpha(readyOverlayAlpha);

            if (phoneOrderAutoSpawner != null)
                phoneOrderAutoSpawner.enabled = false;

            if (shopObject != null)
                shopObject.SetActive(true);

            if (startObject != null)
                startObject.SetActive(true);

            if (playerAnimationController != null)
                playerAnimationController.SetMasked(false);

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

            if (playerAnimationController != null)
                playerAnimationController.SetMasked(true);

            Debug.Log("[FSM] Play 상태 복원");
        }
        else if (currentState == GameFlowState.Result)
        {
            isTimeRunning = false;
            UpdateTimeUI();
            SetDayNightOverlayAlpha(resultOverlayAlpha);

            if (phoneOrderAutoSpawner != null)
                phoneOrderAutoSpawner.enabled = false;

            if (shopObject != null)
                shopObject.SetActive(false);

            if (startObject != null)
                startObject.SetActive(false);

            if (resultPanelUI != null)
                resultPanelUI.OpenResult();

            if (playerAnimationController != null)
                playerAnimationController.SetMasked(true);

            Debug.Log("[FSM] Result 상태 복원");
        }
        else if (currentState == GameFlowState.NextDay)
        {
            EnterReady();
        }
    }
}