using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeShopButton : MonoBehaviour
{
    [Header("강화 대상")]
    [SerializeField] private UpgradeStationType stationType;

    [Header("가격 설정")]
    [SerializeField] private int basePrice = 500;
    [SerializeField] private int priceIncrease = 500;
    [SerializeField] private int maxLevel = 5;

    [Header("UI")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    [Header("돈 관리")]
    [SerializeField] private GameStatusUI gameStatusUI;

    private int currentLevel = 0;

    private int CurrentPrice
    {
        get
        {
            return basePrice + (currentLevel * priceIncrease);
        }
    }

    private void Start()
    {
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(BuyUpgrade);

        RefreshUI();
    }

    public void BuyUpgrade()
    {
        if (currentLevel >= maxLevel)
        {
            Debug.Log("[UpgradeShopButton] 이미 최대 강화입니다.");
            return;
        }

        if (gameStatusUI == null)
        {
            Debug.LogWarning("[UpgradeShopButton] GameStatusUI가 연결되지 않았습니다.");
            return;
        }

        int price = CurrentPrice;

        if (!gameStatusUI.TrySpendGold(price))
        {
            Debug.Log("[UpgradeShopButton] 골드가 부족합니다.");
            return;
        }

        currentLevel++;

        ApplyUpgradeEffect();

        Debug.Log($"[UpgradeShopButton] {stationType} 강화 완료: Lv.{currentLevel}");

        RefreshUI();
    }

    private void ApplyUpgradeEffect()
    {
        switch (stationType)
        {
            case UpgradeStationType.Crafting:
                ProductionUpgradeData.CraftingStartBonus = currentLevel;
                break;

            case UpgradeStationType.Print:
                ProductionUpgradeData.PrintStartBonus = currentLevel;
                break;

            case UpgradeStationType.Packing:
                ProductionUpgradeData.PackingStartBonus = currentLevel;
                break;
        }
    }

    private void RefreshUI()
    {
        if (levelText != null)
            levelText.text = "Lv." + currentLevel;

        if (priceText != null)
        {
            if (currentLevel >= maxLevel)
                priceText.text = "MAX";
            else
                priceText.text = CurrentPrice.ToString() + "G";
        }

        if (descriptionText != null)
        {
            descriptionText.text =
                "강화할 때마다 미니게임 시작 진행도가 1칸 증가합니다.";
        }

        if (upgradeButton != null)
            upgradeButton.interactable = currentLevel < maxLevel;
    }
}