using UnityEngine;

public class PackagingStation : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject packagingPanel;

    private void Start()
    {
        if (packagingPanel != null)
            packagingPanel.SetActive(false);
    }

    private void Update()
    {
        if (packagingPanel != null &&
            packagingPanel.activeSelf &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            packagingPanel.SetActive(false);
        }
    }

    public void OnInteract(GameObject interactor)
    {
        if (packagingPanel == null)
        {
            Debug.LogWarning("[PackagingStation] 포장 패널 연결 안 됨");
            return;
        }

        packagingPanel.SetActive(true);
        Debug.Log("[PackagingStation] 포장 패널 열림");
    }

    public string GetInteractionName()
    {
        return "포장대 사용하기";
    }
}