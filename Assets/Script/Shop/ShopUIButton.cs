using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ShopUIButton : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private ShopEntry entry;
    [SerializeField] private int count = 1;

    public void Buy()
    {
        if (shop != null && entry != null && entry.item != null)
        {
            shop.BuyById(entry.item.id, count);
            Debug.Log($"인벤토리 보유 수량: {Inventory.instance.GetCount(entry.item)}");
            Debug.Log($"{entry.item.itemName} 구매 완료, 현재 개수: {Inventory.instance.GetCount(entry.item)}");
        }
    }

}
