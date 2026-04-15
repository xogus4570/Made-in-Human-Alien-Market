using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private GameStatusUI gameStatus;
    [SerializeField] private CraftingTable craftingTable;

    private IWalletService wallet;

    private void Awake()
    {
        if (gameStatus == null)
            Debug.LogWarning("[Shop] GameStatusUI가 연결되지 않았습니다.", this);

        wallet = new WalletFromGameStatus(gameStatus);
    }

    public bool BuyItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("[Shop] item이 null입니다.", this);
            return false;
        }

        if (!item.soldInShop)
        {
            Debug.LogWarning($"[Shop] 상점 판매 대상이 아닙니다: {item.itemName}", this);
            return false;
        }

        if (wallet == null)
        {
            Debug.LogWarning("[Shop] wallet이 없습니다.", this);
            return false;
        }

        if (!wallet.TryPay(item.shopPrice))
        {
            Debug.Log($"[Shop] 구매 실패(골드 부족): {item.itemName}", this);
            return false;
        }

        bool success = false;

        switch (item.itemType)
        {
            case ItemType.Material:
            case ItemType.Gem:
                success = BuyMaterialOrGem(item);
                break;

            case ItemType.Goods:
                success = BuyGoods(item);
                break;
        }

        if (!success)
        {
            Refund(item.shopPrice);
            Debug.LogWarning($"[Shop] 구매 실패로 환불: {item.itemName}", this);
        }

        return success;
    }

    public bool BuyById(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("[Shop] itemId가 비어 있습니다.", this);
            return false;
        }

        if (ItemDataBase.instance == null)
        {
            Debug.LogWarning("[Shop] ItemDataBase.instance가 없습니다.", this);
            return false;
        }

        Item item = ItemDataBase.instance.GetById(itemId);
        if (item == null)
        {
            Debug.LogWarning($"[Shop] itemId '{itemId}'를 찾지 못했습니다.", this);
            return false;
        }

        return BuyItem(item);
    }

    public bool BuyUpgrade(string upgradeId, int price)
    {
        if (wallet == null)
        {
            Debug.LogWarning("[Shop] wallet이 없습니다.", this);
            return false;
        }

        if (!wallet.TryPay(price))
        {
            Debug.Log($"[Shop] 강화 구매 실패(골드 부족): {upgradeId}", this);
            return false;
        }

        if (craftingTable == null)
        {
            Debug.LogWarning("[Shop] CraftingTable이 연결되지 않았습니다.", this);
            Refund(price);
            return false;
        }

        bool success = craftingTable.ApplyUpgrade(upgradeId);

        if (!success)
            Refund(price);

        return success;
    }

    private bool BuyMaterialOrGem(Item item)
    {
        if (Inventory.instance == null)
        {
            Debug.LogWarning("[Shop] Inventory.instance가 없습니다.", this);
            return false;
        }

        int addCount = Mathf.Max(1, item.shopCount);
        Inventory.instance.Add(item, addCount);

        Debug.Log($"[Shop] 구매 완료: {item.itemName} x{addCount}", this);
        return true;
    }

    private bool BuyGoods(Item item)
    {
        if (GoodsUnlockManager.Instance == null)
        {
            Debug.LogWarning("[Shop] GoodsUnlockManager.Instance가 없습니다.", this);
            return false;
        }

        bool unlocked = GoodsUnlockManager.Instance.Unlock(item.id);
        if (!unlocked)
            return false;

        Debug.Log($"[Shop] 굿즈 해금 완료: {item.itemName}", this);
        return true;
    }

    private void Refund(int amount)
    {
        if (gameStatus != null && amount > 0)
            gameStatus.EarnGold(amount);
    }
}