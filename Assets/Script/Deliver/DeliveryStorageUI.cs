using System.Collections.Generic;
using UnityEngine;

public class DeliveryStorageUI : MonoBehaviour
{
    public static DeliveryStorageUI Instance;

    [SerializeField] private Transform storageGridRoot;

    private InventorySlotUI[] uiSlots;
    private Dictionary<Item, int> storage = new Dictionary<Item, int>();

    private void Awake()
    {
        Instance = this;

        if (storageGridRoot != null)
            uiSlots = storageGridRoot.GetComponentsInChildren<InventorySlotUI>(true);

        Debug.Log($"[StorageUI] Awake - uiSlots: {(uiSlots == null ? -1 : uiSlots.Length)}");
    }

    // 핵심: bool 반환
    public bool AddItemToStorage(Item item, int count)
    {
        if (item == null)
        {
            Debug.Log("[StorageUI] AddItemToStorage 실패: item null");
            return false;
        }

        if (storage.ContainsKey(item))
            storage[item] += count;
        else
            storage[item] = count;

        Debug.Log($"[StorageUI] 추가됨: {item.itemName} x{storage[item]}");

        RefreshUI();
        return true;
    }

    public void RemoveItem(Item item, int count)
    {
        if (!storage.ContainsKey(item)) return;

        storage[item] -= count;

        if (storage[item] <= 0)
            storage.Remove(item);

        RefreshUI();
    }

    // [추가] 창고에 특정 아이템이 몇 개 있는지
    public int GetStoredCount(Item item)
    {
        if (item == null) return 0;
        if (storage.TryGetValue(item, out int count))
            return count;

        return 0;
    }

    // [추가] 창고에서 차감 시도
    public bool TryRemoveFromStorage(Item item, int count)
    {
        if (item == null || count <= 0) return false;
        if (!storage.ContainsKey(item)) return false;
        if (storage[item] < count) return false;

        storage[item] -= count;

        if (storage[item] <= 0)
            storage.Remove(item);

        RefreshUI();
        return true;
    }

    // [추가] 납품하기 버튼에서 호출할 함수
    public void DeliverAll()
    {
        if (OrderManager.Instance == null)
        {
            Debug.LogWarning("[StorageUI] OrderManager.Instance가 없습니다.");
            return;
        }

        int deliveredCount = OrderManager.Instance.TryDeliverAllFromStorage();

        Debug.Log($"[StorageUI] 일괄 납품 완료: {deliveredCount}개 주문 처리");
    }


    private void RefreshUI()
    {
        if (uiSlots == null)
        {
            Debug.Log("[StorageUI] RefreshUI 실패: uiSlots null");
            return;
        }

        // 전체 초기화
        foreach (var slot in uiSlots)
            slot.Clear();

        int index = 0;

        foreach (var pair in storage)
        {
            if (index >= uiSlots.Length) break;

            uiSlots[index].SetItem(pair.Key, pair.Value);
            index++;
        }

        Debug.Log($"[StorageUI] UI 갱신 완료 (storageCount={storage.Count})");
    }
}