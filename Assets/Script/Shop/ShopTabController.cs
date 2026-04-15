using UnityEngine;
using UnityEngine.UI;

public class ShopTabController : MonoBehaviour
{
    [Header("탭 버튼")]
    [SerializeField] private Button materialTabButton;
    [SerializeField] private Button upgradeTabButton;
    [SerializeField] private Button goodsTabButton;

    [Header("탭 외형")]
    [SerializeField] private ShopTabButtonUI materialTabVisual;
    [SerializeField] private ShopTabButtonUI upgradeTabVisual;
    [SerializeField] private ShopTabButtonUI goodsTabVisual;

    [Header("스크롤")]
    [SerializeField] private ScrollRect scrollRect;

    [Header("콘텐츠")]
    [SerializeField] private RectTransform materialContent;
    [SerializeField] private RectTransform upgradeContent;
    [SerializeField] private RectTransform goodsContent;

    private ShopTabType currentTab;

    private Vector2 materialStartPos;
    private Vector2 upgradeStartPos;
    private Vector2 goodsStartPos;

    private void Awake()
    {
        if (materialContent != null) materialStartPos = materialContent.anchoredPosition;
        if (upgradeContent != null) upgradeStartPos = upgradeContent.anchoredPosition;
        if (goodsContent != null) goodsStartPos = goodsContent.anchoredPosition;
    }

    private void Start()
    {
        if (materialTabButton != null)
            materialTabButton.onClick.AddListener(OpenMaterialTab);

        if (upgradeTabButton != null)
            upgradeTabButton.onClick.AddListener(OpenUpgradeTab);

        if (goodsTabButton != null)
            goodsTabButton.onClick.AddListener(OpenGoodsTab);

        OpenMaterialTab();
    }

    public void OpenMaterialTab()
    {
        currentTab = ShopTabType.Material;
        RefreshContent(true);
        RefreshTabButtonVisual();
    }

    public void OpenUpgradeTab()
    {
        currentTab = ShopTabType.Upgrade;
        RefreshContent(true);
        RefreshTabButtonVisual();
    }

    public void OpenGoodsTab()
    {
        currentTab = ShopTabType.Goods;
        RefreshContent(true);
        RefreshTabButtonVisual();
    }

    public void ResetCurrentTabPosition()
    {
        RectTransform targetContent = GetCurrentContent();
        if (targetContent == null) return;

        targetContent.anchoredPosition = GetStartPosition(targetContent);

        if (scrollRect != null)
            scrollRect.content = targetContent;
    }

    private void RefreshContent(bool resetPosition)
    {
        if (materialContent != null)
            materialContent.gameObject.SetActive(currentTab == ShopTabType.Material);

        if (upgradeContent != null)
            upgradeContent.gameObject.SetActive(currentTab == ShopTabType.Upgrade);

        if (goodsContent != null)
            goodsContent.gameObject.SetActive(currentTab == ShopTabType.Goods);

        RectTransform targetContent = GetCurrentContent();

        if (scrollRect != null && targetContent != null)
        {
            scrollRect.content = targetContent;

            if (resetPosition)
                targetContent.anchoredPosition = GetStartPosition(targetContent);

            LayoutRebuilder.ForceRebuildLayoutImmediate(targetContent);
            Canvas.ForceUpdateCanvases();
        }
    }

    private RectTransform GetCurrentContent()
    {
        switch (currentTab)
        {
            case ShopTabType.Material:
                return materialContent;
            case ShopTabType.Upgrade:
                return upgradeContent;
            case ShopTabType.Goods:
                return goodsContent;
        }

        return null;
    }

    private Vector2 GetStartPosition(RectTransform target)
    {
        if (target == materialContent) return materialStartPos;
        if (target == upgradeContent) return upgradeStartPos;
        if (target == goodsContent) return goodsStartPos;

        return Vector2.zero;
    }

    private void RefreshTabButtonVisual()
    {
        if (materialTabVisual != null)
            materialTabVisual.SetSelected(currentTab == ShopTabType.Material);

        if (upgradeTabVisual != null)
            upgradeTabVisual.SetSelected(currentTab == ShopTabType.Upgrade);

        if (goodsTabVisual != null)
            goodsTabVisual.SetSelected(currentTab == ShopTabType.Goods);
    }
}