using UnityEngine;

public class InspectorSpawnManager : MonoBehaviour
{
    [Header("조건 확인")]
    [SerializeField] private GameStatusUI gameStatusUI;
    [SerializeField] private int satisfactionThreshold = 50;

    [Header("검문관")]
    [SerializeField] private GameObject inspectorObject;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform player;

    [Header("등장 설정")]
    [SerializeField] private bool hideWhenConditionRecovered = false;

    private bool hasSpawned;

    private void Start()
    {
        if (inspectorObject != null)
            inspectorObject.SetActive(false);
    }

    private void Update()
    {
        if (gameStatusUI == null) return;
        if (inspectorObject == null) return;
        if (GameFlowController.Instance == null) return;

        bool isPlayState = GameFlowController.Instance.currentState == GameFlowState.Play;
        bool isLowSatisfaction = gameStatusUI.CurrentSatisfaction <= satisfactionThreshold;

        if (isPlayState && isLowSatisfaction)
        {
            SpawnInspector();
        }
        else if (hideWhenConditionRecovered)
        {
            HideInspector();
        }
    }

    private void SpawnInspector()
    {
        if (hasSpawned && inspectorObject.activeSelf)
            return;

        if (spawnPoint != null)
            inspectorObject.transform.position = spawnPoint.position;

        InspectorController controller = inspectorObject.GetComponent<InspectorController>();
        if (controller != null && player != null)
            controller.SetPlayer(player);

        inspectorObject.SetActive(true);
        hasSpawned = true;

        Debug.Log("[InspectorSpawnManager] 고객만족도 저하로 검문관 등장");
    }

    private void HideInspector()
    {
        if (!hasSpawned) return;

        inspectorObject.SetActive(false);
        hasSpawned = false;

        Debug.Log("[InspectorSpawnManager] 검문관 비활성화");
    }
}