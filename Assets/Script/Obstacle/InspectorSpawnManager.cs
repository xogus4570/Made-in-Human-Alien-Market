using UnityEngine;

public class InspectorSpawnManager : MonoBehaviour
{
    [Header("연결")]
    [SerializeField] private GameStatusUI gameStatusUI;
    [SerializeField] private GameObject inspectorObject;

    [Header("등장 조건")]
    [SerializeField] private int satisfactionThreshold = 50;

    [Header("설정")]
    [SerializeField] private bool onlyInPlayState = true;
    [SerializeField] private bool hideWhenConditionRecovered = true;

    private bool wasInspectorActive = false;

    private void Awake()
    {
        ForceHideInspectorOnSceneLoad();
    }

    private void Start()
    {
        ApplyInspectorStateByCondition();
    }

    private void Update()
    {
        ApplyInspectorStateByCondition();
    }

    private void ApplyInspectorStateByCondition()
    {
        if (gameStatusUI == null || inspectorObject == null)
            return;

        if (onlyInPlayState)
        {
            if (GameFlowController.Instance == null ||
                GameFlowController.Instance.currentState != GameFlowState.Play)
            {
                HideInspector();
                return;
            }
        }

        int currentSatisfaction = gameStatusUI.CurrentSatisfaction;
        bool shouldAppear = currentSatisfaction <= satisfactionThreshold;

        if (shouldAppear)
        {
            ShowInspector();
        }
        else
        {
            // 고객만족도가 기준보다 높으면 옵션과 상관없이 무조건 퇴장
            HideInspector();
        }
    }

    private void ShowInspector()
    {
        if (wasInspectorActive && inspectorObject.activeSelf)
        {
            SaveInspectorPosition();
            return;
        }

        if (GameDataManager.Instance != null &&
            GameDataManager.Instance.hasInspectorPosition)
        {
            inspectorObject.transform.position = GameDataManager.Instance.inspectorPosition;
        }

        inspectorObject.SetActive(true);

        SpriteRenderer spriteRenderer = inspectorObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        wasInspectorActive = true;
        SaveInspectorState(true);

        Debug.Log("[InspectorSpawnManager] 고객만족도 저하로 검문관 등장");
    }

    private void HideInspector()
    {
        if (inspectorObject == null)
            return;

        if (inspectorObject.activeSelf)
        {
            inspectorObject.SetActive(false);
            Debug.Log("[InspectorSpawnManager] 고객만족도 회복 또는 Play 상태 종료로 검문관 퇴장");
        }

        wasInspectorActive = false;
        SaveInspectorState(false);
    }

    private void ForceHideInspectorOnSceneLoad()
    {
        if (inspectorObject == null)
            return;

        inspectorObject.SetActive(false);
        wasInspectorActive = false;

        if (GameDataManager.Instance != null)
            GameDataManager.Instance.inspectorWasActive = false;
    }

    private void SaveInspectorPosition()
    {
        if (GameDataManager.Instance == null || inspectorObject == null)
            return;

        if (!inspectorObject.activeSelf)
            return;

        GameDataManager.Instance.inspectorPosition = inspectorObject.transform.position;
        GameDataManager.Instance.hasInspectorPosition = true;
    }

    private void SaveInspectorState(bool active)
    {
        if (GameDataManager.Instance == null)
            return;

        GameDataManager.Instance.inspectorWasActive = active;

        if (active)
            SaveInspectorPosition();
    }
}