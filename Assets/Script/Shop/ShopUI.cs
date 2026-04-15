using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject shopRoot;
    [SerializeField] private bool startOpened = false;
    [SerializeField] private ShopTabController tabController;

    private bool isOpen;

    private void Awake()
    {
        isOpen = startOpened;
        ApplyState();
    }

    public void ToggleShop()
    {
        isOpen = !isOpen;
        ApplyState();

        if (isOpen && tabController != null)
            tabController.ResetCurrentTabPosition();
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
    }
}