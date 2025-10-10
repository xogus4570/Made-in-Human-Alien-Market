using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private ShopEntry[] catalog;
    [SerializeField] private GameStatusUI gameStatus;
    private IWalletService wallet;

    private void Awake()
    {
        if (gameStatus == null)
            Debug.LogWarning("[Shop] GameStatusUI가 연결되지 않았습니다.");
        wallet = new WalletFromGameStatus(gameStatus);
    }


    public bool BuyById(string itemId, int count = 1)
    {
        // 입력값 방어
        if (string.IsNullOrEmpty(itemId) || count <= 0) return false;

        var entry = FindEntryById(itemId);
        if (entry == null) return false;

        int cost = entry.buyPrice * Mathf.Max(1, count);
        if (!wallet.TryPay(cost)) return false;

        // DB 가드 + 환불
        if (ItemDataBase.instance == null)
        {
            wallet.Add(cost); // 환불
            Debug.LogError("[Shop] ItemDataBase.instance 없음 → 환불");
            return false;
        }

        var item = ItemDataBase.instance.GetById(itemId);
        if (item == null)
        {
            wallet.Add(cost); // 환불
            Debug.LogError($"[Shop] ItemDatabase에서 {itemId} 없음 → 환불");
            return false;
        }

        // 인벤토리 가드 + 환불
        if (Inventory.instance == null)
        {
            wallet.Add(cost); // 환불
            Debug.LogError("[Shop] Inventory.instance 없음 → 환불");
            return false;
        }

        // 여기까지 왔으면 구매 성공
        Debug.Log($"[Shop-Debug] DB에서 '{item.itemName}'(ID:{itemId}) 아이템을 찾았습니다. 이미지 상태: {item.itemImage}");
        Inventory.instance.Add(item, count);
        Debug.Log($"{item.itemName} 구매 완료");

        return true;
    }

    private ShopEntry FindEntryById(string id)
    {
        foreach (var e in catalog)
            if (e != null && e.item != null && e.item.id == id) return e;
        return null;
    }
}
