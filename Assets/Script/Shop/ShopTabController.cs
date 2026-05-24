using UnityEngine;
using UnityEngine.UI;

public class ShopTabController : MonoBehaviour
{
    [Header("Scroll Rect")]
    [SerializeField] private ScrollRect scrollRect;

    [Header("탭 Content")]
    [SerializeField] private RectTransform materialContent;
    [SerializeField] private RectTransform gemContent;
    [SerializeField] private RectTransform upgradeContent;
    [SerializeField] private RectTransform goodsContent;

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

    private void RefreshTab()
    {
        if (materialContent != null)
            materialContent.gameObject.SetActive(currentTab == ShopTabType.Material);

        if (gemContent != null)
            gemContent.gameObject.SetActive(currentTab == ShopTabType.Gem);

        if (upgradeContent != null)
            upgradeContent.gameObject.SetActive(currentTab == ShopTabType.Upgrade);

        if (goodsContent != null)
            goodsContent.gameObject.SetActive(currentTab == ShopTabType.Goods);

        if (scrollRect != null)
            scrollRect.content = GetCurrentContent();

        ResetCurrentTabPosition();
        RefreshTabButtonVisual();
    }

    private RectTransform GetCurrentContent()
    {
        switch (currentTab)
        {
            case ShopTabType.Material:
                return materialContent;
            case ShopTabType.Gem:
                return gemContent;
            case ShopTabType.Upgrade:
                return upgradeContent;
            case ShopTabType.Goods:
                return goodsContent;
        }

        return materialContent;
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
        RectTransform content = GetCurrentContent();

        if (content == null)
            return;

        Vector2 pos = content.anchoredPosition;
        pos.x = 0f;
        content.anchoredPosition = pos;
    }
}