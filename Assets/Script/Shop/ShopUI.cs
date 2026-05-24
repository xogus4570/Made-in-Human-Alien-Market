using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [Header("상점 전체 UI")]
    [SerializeField] private GameObject shopRoot;

    [Header("상점 여는 버튼")]
    [SerializeField] private GameObject openShopButton;

    [Header("시작 시 상점 열림 여부")]
    [SerializeField] private bool startOpened = false;

    [Header("탭 컨트롤러")]
    [SerializeField] private ShopTabController tabController;

    private bool isOpen;

    private void Awake()
    {
        isOpen = startOpened;
        ApplyState();
    }

    public void OpenShop()
    {
        isOpen = true;
        ApplyState();

        if (tabController != null)
            tabController.ResetCurrentTabPosition();
    }

    public void CloseShop()
    {
        isOpen = false;
        ApplyState();
    }

    private void ApplyState()
    {
        if (shopRoot != null)
            shopRoot.SetActive(isOpen);

        if (openShopButton != null)
            openShopButton.SetActive(!isOpen);
    }
}