using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class ShopUIButton : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private List<ShopEntry> entries = new List<ShopEntry>(); //여러 개의 아이템 엔트리
    [SerializeField] private int count = 1; // 기본 구매 수량 (공통 수량)

    public void Buy()
    {
        if (shop == null || entries == null || entries.Count == 0)
        {
            Debug.LogWarning("Shop 또는 Entries가 비어 있습니다.");
            return;
        }

        foreach (var entry in entries)
        {
            if (entry == null || entry.item == null)
                continue;

            bool success = shop.BuyById(entry.item.id, count);

            if (success)
            {
                Debug.Log($"인벤토리 보유 수량: {Inventory.instance.GetCount(entry.item)}");
                Debug.Log($"{entry.item.itemName} 구매 완료, 현재 개수: {Inventory.instance.GetCount(entry.item)}");
            }
            else
            {
                Debug.Log($"구매 실패: {entry.item.itemName} (골드 부족 또는 세팅 오류)");
            }
        }
    }
}