using UnityEngine;

public class ShopTabController : MonoBehaviour
{
    [Header("탭 Content")]
    [SerializeField] private GameObject materialContent;
    [SerializeField] private GameObject gemContent;
    [SerializeField] private GameObject upgradeContent;
    [SerializeField] private GameObject goodsContent;

    [Header("탭 버튼 외형")]
    [SerializeField] private ShopTabButtonUI materialTabVisual;
    [SerializeField] private ShopTabButtonUI gemTabVisual;
    [SerializeField] private ShopTabButtonUI upgradeTabVisual;
    [SerializeField] private ShopTabButtonUI goodsTabVisual;

    [Header("처음 열릴 탭")]
    [SerializeField] private ShopTabType currentTab = ShopTabType.Material;

    private void Awake()
    {
        RefreshTab();
    }

    public void OpenMaterialTab()
    {
        currentTab = ShopTabType.Material;
        RefreshTab();
    }

    public void OpenGemTab()
    {
        currentTab = ShopTabType.Gem;
        RefreshTab();
    }

    public void OpenUpgradeTab()
    {
        currentTab = ShopTabType.Upgrade;
        RefreshTab();
    }

    public void OpenGoodsTab()
    {
        currentTab = ShopTabType.Goods;
        RefreshTab();
    }

    public void RefreshTab()
    {
        if (materialContent != null)
            materialContent.SetActive(currentTab == ShopTabType.Material);

        if (gemContent != null)
            gemContent.SetActive(currentTab == ShopTabType.Gem);

        if (upgradeContent != null)
            upgradeContent.SetActive(currentTab == ShopTabType.Upgrade);

        if (goodsContent != null)
            goodsContent.SetActive(currentTab == ShopTabType.Goods);

        RefreshTabButtonVisual();
    }

    private void RefreshTabButtonVisual()
    {
        if (materialTabVisual != null)
            materialTabVisual.SetSelected(currentTab == ShopTabType.Material);

        if (gemTabVisual != null)
            gemTabVisual.SetSelected(currentTab == ShopTabType.Gem);

        if (upgradeTabVisual != null)
            upgradeTabVisual.SetSelected(currentTab == ShopTabType.Upgrade);

        if (goodsTabVisual != null)
            goodsTabVisual.SetSelected(currentTab == ShopTabType.Goods);
    }

    public void ResetCurrentTabPosition()
    {
        ResetContentPosition(materialContent);
        ResetContentPosition(gemContent);
        ResetContentPosition(upgradeContent);
        ResetContentPosition(goodsContent);
    }

    private void ResetContentPosition(GameObject contentObject)
    {
        if (contentObject == null)
            return;

        RectTransform rect = contentObject.GetComponent<RectTransform>();

        if (rect == null)
            return;

        Vector2 pos = rect.anchoredPosition;
        pos.y = 443.9999f;
        rect.anchoredPosition = pos;
    }
}