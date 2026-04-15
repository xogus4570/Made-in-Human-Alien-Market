using UnityEngine;

public class ShopUpgradeButton : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private string upgradeId;
    [SerializeField] private int price = 100;

    public void BuyUpgrade()
    {
        if (shop == null)
        {
            Debug.LogWarning("[ShopUpgradeButton] Shop이 연결되지 않았습니다.");
            return;
        }

        if (string.IsNullOrEmpty(upgradeId))
        {
            Debug.LogWarning("[ShopUpgradeButton] upgradeId가 비어 있습니다.");
            return;
        }

        bool success = shop.BuyUpgrade(upgradeId, price);

        if (success)
            Debug.Log($"[ShopUpgradeButton] 강화 구매 성공: {upgradeId}");
        else
            Debug.Log($"[ShopUpgradeButton] 강화 구매 실패: {upgradeId}");
    }
}   