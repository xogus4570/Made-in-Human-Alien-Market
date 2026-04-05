using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ShopUIButton : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private ShopEntry entry;
    [SerializeField] private int count = 1; // 기본 구매 수량 (공통 수량)

    public void Buy()
    {
        if (shop == null || entry == null || entry.item == null)
        {
            Debug.LogWarning("Shop 또는 Entry가 비어 있습니다.");
            return;
        }

        bool success = shop.BuyById(entry.item.id, count);

        if (success)
        {
            Debug.Log($"{entry.item.itemName} 구매 완료, 현재 개수: {Inventory.instance.GetCount(entry.item)}");
        }
        else
        {
            Debug.Log($"구매 실패: {entry.item.itemName} (골드 부족 또는 세팅 오류)");
        }
    }
}