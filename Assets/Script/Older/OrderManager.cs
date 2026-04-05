using System;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    [SerializeField] private GameStatusUI gameStatusUI;

    public static OrderManager Instance;

    public event Action OnOrderListChanged;

    private readonly List<OrderData> activeOrders = new List<OrderData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void AddOrder(
        Item item,
        int quantity,
        int rewardGold,
        int rewardExp,
        int rewardInfluence,
        int rewardSatisfaction)
    {
        if (item == null)
        {
            Debug.LogWarning("[OrderManager] item is null");
            return;
        }

        if (quantity <= 0)
        {
            Debug.LogWarning("[OrderManager] quantity must be greater than 0");
            return;
        }

        OrderData newOrder = new OrderData(
            item,
            quantity,
            rewardGold,
            rewardExp,
            rewardInfluence,
            rewardSatisfaction
        );

        activeOrders.Add(newOrder);

        Debug.Log($"[OrderManager] 주문 추가: {item.itemName} x{quantity}");
        OnOrderListChanged?.Invoke();
    }

    public void CompleteOrder(string orderId)
    {
        OrderData order = activeOrders.Find(o => o.orderId == orderId);
        if (order == null)
        {
            Debug.LogWarning($"[OrderManager] 주문을 찾을 수 없음: {orderId}");
            return;
        }

        order.isCompleted = true;
        activeOrders.Remove(order);

        Debug.Log($"[OrderManager] 주문 완료: {order.orderedItem.itemName} x{order.quantity}");
        OnOrderListChanged?.Invoke();
    }

    public IReadOnlyList<OrderData> GetActiveOrders()
    {
        return activeOrders.AsReadOnly();
    }

    public void ClearAllOrders()
    {
        activeOrders.Clear();
        OnOrderListChanged?.Invoke();
    }

    // 기존 인벤토리 직접 납품 방식 (지금 구조에선 거의 안 써도 됨)
    public bool TryDeliver(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("[Delivery] item is null");
            return false;
        }

        if (Inventory.instance == null)
        {
            Debug.LogWarning("[Delivery] Inventory.instance가 없습니다.");
            return false;
        }

        int owned = Inventory.instance.GetCount(item);

        if (owned <= 0)
        {
            Debug.Log("[Delivery] 인벤토리에 없음");
            return false;
        }

        for (int i = 0; i < activeOrders.Count; i++)
        {
            var order = activeOrders[i];

            if (order.orderedItem == null)
                continue;

            if (order.orderedItem.id != item.id)
                continue;

            if (owned < order.quantity)
            {
                Debug.Log("[Delivery] 수량 부족");
                return false;
            }

            bool removed = Inventory.instance.Remove(item, order.quantity);
            if (!removed)
            {
                Debug.Log("[Delivery] Remove 실패");
                return false;
            }

            activeOrders.RemoveAt(i);

            if (DailyResultManager.Instance != null)
            {
                DailyResultManager.Instance.AddReward(
                    order.rewardGold,
                    order.rewardExp,
                    order.rewardInfluence,
                    order.rewardSatisfaction
                );
            }
            else
            {
                Debug.LogWarning("[OrderManager] DailyResultManager.Instance가 없습니다.");
            }

            OnOrderListChanged?.Invoke();

            Debug.Log($"[Delivery] 납품 성공: {item.itemName}");
            return true;
        }

        Debug.Log("[Delivery] 해당 주문 없음");
        return false;
    }

    // 창고 전체 기준 일괄 납품
    public int TryDeliverAllFromStorage()
    {
        if (DeliveryStorageUI.Instance == null)
        {
            Debug.LogWarning("[Delivery] DeliveryStorageUI.Instance가 없습니다.");
            return 0;
        }

        int deliveredCount = 0;

        // 뒤에서부터 제거해야 안전함
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            var order = activeOrders[i];

            if (order == null || order.orderedItem == null)
                continue;

            int storedCount = DeliveryStorageUI.Instance.GetStoredCount(order.orderedItem);

            if (storedCount < order.quantity)
                continue;

            bool removedFromStorage = DeliveryStorageUI.Instance.TryRemoveFromStorage(
                order.orderedItem,
                order.quantity
            );

            if (!removedFromStorage)
                continue;

            activeOrders.RemoveAt(i);

            if (DailyResultManager.Instance != null)
            {
                DailyResultManager.Instance.AddReward(
                    order.rewardGold,
                    order.rewardExp,
                    order.rewardInfluence,
                    order.rewardSatisfaction
                );
            }
            else
            {
                Debug.LogWarning("[OrderManager] DailyResultManager.Instance가 없습니다.");
            }

            deliveredCount++;

            Debug.Log($"[Delivery] 납품 성공: {order.orderedItem.itemName} x{order.quantity}");
        }

        if (deliveredCount > 0)
            OnOrderListChanged?.Invoke();

        return deliveredCount;
    }
}