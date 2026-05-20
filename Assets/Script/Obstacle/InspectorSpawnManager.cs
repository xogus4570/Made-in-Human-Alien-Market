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

    private void Start()
    {
        RestoreInspectorState();
    }

    private void Update()
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

        bool shouldAppear = gameStatusUI.CurrentSatisfaction <= satisfactionThreshold;

        if (shouldAppear)
        {
            ShowInspector();
        }
        else
        {
            if (hideWhenConditionRecovered)
                HideInspector();
        }
    }

    private void RestoreInspectorState()
    {
        if (inspectorObject == null)
            return;

        if (GameDataManager.Instance != null &&
            GameDataManager.Instance.inspectorWasActive &&
            GameDataManager.Instance.hasInspectorPosition)
        {
            inspectorObject.transform.position = GameDataManager.Instance.inspectorPosition;
            inspectorObject.SetActive(true);

            SpriteRenderer spriteRenderer = inspectorObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;

            wasInspectorActive = true;

            Debug.Log("[InspectorSpawnManager] 저장된 검문관 상태 복원");
        }
        else
        {
            inspectorObject.SetActive(false);
            wasInspectorActive = false;
        }
    }

    private void ShowInspector()
    {
        if (wasInspectorActive && inspectorObject.activeSelf)
            return;

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
            SaveInspectorPosition();

        if (inspectorObject.activeSelf)
            inspectorObject.SetActive(false);

        wasInspectorActive = false;
        SaveInspectorState(false);
    }

    private void SaveInspectorPosition()
    {
        if (GameDataManager.Instance == null || inspectorObject == null)
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