using UnityEngine;

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
        if (currentState == GameFlowState.Ready && Input.GetKeyDown(KeyCode.S))
        {
            EnterPlay();
        }

        if (currentState == GameFlowState.Play && Input.GetKeyDown(KeyCode.R))
        {
            EnterResult();
        }
    }

    public void EnterReady()
    {
        currentState = GameFlowState.Ready;

        if (GameDataManager.Instance != null)
            GameDataManager.Instance.flowState = GameFlowState.Ready;

        if (phoneOrderAutoSpawner != null)
            phoneOrderAutoSpawner.enabled = false;

        if (shopObject != null)
            shopObject.SetActive(true);

        Debug.Log("[FSM] Ready 상태: 준비 단계 / 상점 이용 가능 / 주문 생성 OFF");
    }

    public void EnterPlay()
    {
        currentState = GameFlowState.Play;

        if (GameDataManager.Instance != null)
            GameDataManager.Instance.flowState = GameFlowState.Play;

        if (phoneOrderAutoSpawner != null)
            phoneOrderAutoSpawner.enabled = true;

        if (shopObject != null)
            shopObject.SetActive(false);

        Debug.Log("[FSM] Play 상태: 영업 시작 / 상점 이용 불가 / 주문 생성 ON");
    }

    public void EnterResult()
    {
        currentState = GameFlowState.Result;

        if (GameDataManager.Instance != null)
            GameDataManager.Instance.flowState = GameFlowState.Result;

        if (phoneOrderAutoSpawner != null)
            phoneOrderAutoSpawner.enabled = false;

        if (shopObject != null)
            shopObject.SetActive(false);

        Debug.Log("[FSM] Result 상태: 결과창 표시 / 주문 생성 OFF");

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

    private void ApplyStateWithoutTransition()
    {
        if (currentState == GameFlowState.Ready)
        {
            if (phoneOrderAutoSpawner != null)
                phoneOrderAutoSpawner.enabled = false;

            if (shopObject != null)
                shopObject.SetActive(true);

            Debug.Log("[FSM] Ready 상태 복원");
        }
        else if (currentState == GameFlowState.Play)
        {
            if (phoneOrderAutoSpawner != null)
                phoneOrderAutoSpawner.enabled = true;

            if (shopObject != null)
                shopObject.SetActive(false);

            Debug.Log("[FSM] Play 상태 복원");
        }
        else if (currentState == GameFlowState.Result)
        {
            if (phoneOrderAutoSpawner != null)
                phoneOrderAutoSpawner.enabled = false;

            if (shopObject != null)
                shopObject.SetActive(false);

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