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
        wallet = new WalletFromGameStatus(gameStatus);
    }


    public bool BuyById(string itemId, int count = 1)
    {
        var entry = FindEntryById(itemId);
        if (entry == null) return false;

        int cost = entry.buyPrice * Mathf.Max(1, count);
        if (!gameStatus.TrySpendGold(cost)) return false;

        var item = ItemDataBase.instance.GetById(itemId);

        //#######################
        if (item != null)
        {
            Debug.Log($"[Shop-Debug] DB에서 '{item.itemName}'(ID:{itemId}) 아이템을 찾았습니다. 이미지 상태: {item.itemImage}");
        }
        else
        {
            Debug.LogError($"[Shop-Debug] DB에서 ID:{itemId} 아이템을 찾지 못했습니다!");
        }
        //##########################

        if (item == null)
        {
            Debug.LogError($"[Shop] ItemDatabase에서 {itemId}를 찾을 수 없음!");
            return false;
        }
       

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
