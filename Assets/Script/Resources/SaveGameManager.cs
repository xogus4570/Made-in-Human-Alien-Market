using System;
using System.Collections.Generic;
using UnityEngine;

public static class SaveGameManager
{
    private const string SaveKey = "MIHAM_SAVE_DATA";
    private const string HasSaveKey = "MIHAM_HAS_SAVE";

    private static bool loadRequested = false;

    [Serializable]
    public class SaveData
    {
        public int day;
        public int gold;
        public int influence;
        public int satisfaction;

        public int level;
        public int currentExp;

        public List<ItemSaveData> inventoryItems = new List<ItemSaveData>();
        public List<OrderSaveData> activeOrders = new List<OrderSaveData>();
    }

    [Serializable]
    public class ItemSaveData
    {
        public string itemId;
        public int count;
    }

    [Serializable]
    public class OrderSaveData
    {
        public string orderedItemId;
        public int quantity;

        public int rewardGold;
        public int rewardExp;
        public int rewardInfluence;
        public int rewardSatisfaction;
    }

    public static bool HasSave()
    {
        return PlayerPrefs.GetInt(HasSaveKey, 0) == 1 && PlayerPrefs.HasKey(SaveKey);
    }

    public static void RequestLoadOnNextMainScene()
    {
        loadRequested = true;
        Debug.Log("[이어하기] 메인 씬 로드 후 저장 데이터를 불러오도록 예약했습니다.");
    }

    public static bool ConsumeLoadRequest()
    {
        if (!loadRequested)
            return false;

        loadRequested = false;
        return true;
    }

    public static void Save()
    {
        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning("[세이브] 저장 실패: GameDataManager.Instance가 없습니다.");
            return;
        }

        SaveData data = new SaveData();

        data.day = GameDataManager.Instance.day;
        data.gold = GameDataManager.Instance.gold;
        data.influence = GameDataManager.Instance.influence;
        data.satisfaction = GameDataManager.Instance.satisfaction;

        data.level = GameDataManager.Instance.level;
        data.currentExp = GameDataManager.Instance.currentExp;

        SaveInventory(data);
        SaveOrders(data);

        string json = JsonUtility.ToJson(data, true);

        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.SetInt(HasSaveKey, 1);
        PlayerPrefs.Save();

        Debug.Log($"[세이브] 저장 완료: {data.day}일차 / 인벤토리 {data.inventoryItems.Count}종 / 주문 {data.activeOrders.Count}개");
    }

    public static void Load()
    {
        if (!HasSave())
        {
            Debug.LogWarning("[세이브] 불러오기 실패: 저장 파일이 없습니다.");
            return;
        }

        if (GameDataManager.Instance == null)
        {
            Debug.LogWarning("[세이브] 불러오기 실패: GameDataManager.Instance가 없습니다.");
            return;
        }

        string json = PlayerPrefs.GetString(SaveKey, "");

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning("[세이브] 불러오기 실패: 저장 데이터가 비어 있습니다.");
            return;
        }

        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (data == null)
        {
            Debug.LogWarning("[세이브] 불러오기 실패: 저장 데이터 변환에 실패했습니다.");
            return;
        }

        GameDataManager.Instance.day = Mathf.Clamp(data.day, 1, GameStatusUI.MaxDays);
        GameDataManager.Instance.gold = Mathf.Max(0, data.gold);
        GameDataManager.Instance.influence = Mathf.Max(0, data.influence);
        GameDataManager.Instance.satisfaction = Mathf.Clamp(data.satisfaction, 0, 100);

        GameDataManager.Instance.level = Mathf.Max(1, data.level);
        GameDataManager.Instance.currentExp = Mathf.Max(0, data.currentExp);

        LoadInventory(data);
        LoadOrders(data);

        Debug.Log($"[세이브] 불러오기 완료: {data.day}일차 / 인벤토리 {data.inventoryItems.Count}종 / 주문 {data.activeOrders.Count}개");
    }

    public static void DeleteSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.DeleteKey(HasSaveKey);
        PlayerPrefs.Save();

        Debug.Log("[세이브] 저장 파일을 삭제했습니다.");
    }

    private static void SaveInventory(SaveData data)
    {
        if (Inventory.instance == null)
        {
            Debug.LogWarning("[세이브] 인벤토리 저장 생략: Inventory.instance가 없습니다.");
            return;
        }

        var slots = Inventory.instance.GetAllSlots();

        foreach (var slot in slots)
        {
            if (slot == null)
                continue;

            if (slot.item == null)
                continue;

            if (string.IsNullOrEmpty(slot.item.id))
                continue;

            if (slot.count <= 0)
                continue;

            data.inventoryItems.Add(new ItemSaveData
            {
                itemId = slot.item.id,
                count = slot.count
            });
        }

        Debug.Log($"[세이브] 인벤토리 저장 준비 완료: {data.inventoryItems.Count}종");
    }

    private static void LoadInventory(SaveData data)
    {
        if (Inventory.instance == null)
        {
            Debug.LogWarning("[세이브] 인벤토리 불러오기 생략: Inventory.instance가 없습니다.");
            return;
        }

        if (ItemDataBase.instance == null)
        {
            Debug.LogWarning("[세이브] 인벤토리 불러오기 생략: ItemDataBase.instance가 없습니다.");
            return;
        }

        Inventory.instance.ClearAll();

        if (data.inventoryItems == null)
        {
            Debug.Log("[세이브] 불러올 인벤토리 데이터가 없습니다.");
            return;
        }

        int restoredCount = 0;

        foreach (var savedItem in data.inventoryItems)
        {
            if (savedItem == null)
                continue;

            if (string.IsNullOrEmpty(savedItem.itemId))
                continue;

            if (savedItem.count <= 0)
                continue;

            Item item = ItemDataBase.instance.GetById(savedItem.itemId);

            if (item == null)
            {
                Debug.LogWarning($"[세이브] 인벤토리 복원 실패: ItemDB에서 {savedItem.itemId} 아이템을 찾을 수 없습니다.");
                continue;
            }

            Inventory.instance.Add(item, savedItem.count);
            restoredCount++;
        }

        Debug.Log($"[세이브] 인벤토리 불러오기 완료: {restoredCount}종 복원");
    }

    private static void SaveOrders(SaveData data)
    {
        if (OrderManager.Instance == null)
        {
            Debug.Log("[세이브] 주문 저장 생략: OrderManager.Instance가 없습니다.");
            return;
        }

        var orders = OrderManager.Instance.GetActiveOrders();

        foreach (var order in orders)
        {
            if (order == null)
                continue;

            if (order.orderedItem == null)
                continue;

            if (string.IsNullOrEmpty(order.orderedItem.id))
                continue;

            data.activeOrders.Add(new OrderSaveData
            {
                orderedItemId = order.orderedItem.id,
                quantity = order.quantity,
                rewardGold = order.rewardGold,
                rewardExp = order.rewardExp,
                rewardInfluence = order.rewardInfluence,
                rewardSatisfaction = order.rewardSatisfaction
            });
        }

        Debug.Log($"[세이브] 주문 저장 준비 완료: {data.activeOrders.Count}개");
    }

    private static void LoadOrders(SaveData data)
    {
        if (OrderManager.Instance == null)
        {
            Debug.Log("[세이브] 주문 불러오기 생략: OrderManager.Instance가 없습니다.");
            return;
        }

        if (ItemDataBase.instance == null)
        {
            Debug.LogWarning("[세이브] 주문 불러오기 생략: ItemDataBase.instance가 없습니다.");
            return;
        }

        OrderManager.Instance.ClearAllOrders();

        if (data.activeOrders == null)
        {
            Debug.Log("[세이브] 불러올 주문 데이터가 없습니다.");
            return;
        }

        int restoredCount = 0;

        foreach (var savedOrder in data.activeOrders)
        {
            if (savedOrder == null)
                continue;

            if (string.IsNullOrEmpty(savedOrder.orderedItemId))
                continue;

            Item item = ItemDataBase.instance.GetById(savedOrder.orderedItemId);

            if (item == null)
            {
                Debug.LogWarning($"[세이브] 주문 복원 실패: ItemDB에서 {savedOrder.orderedItemId} 아이템을 찾을 수 없습니다.");
                continue;
            }

            OrderManager.Instance.AddOrder(
                item,
                Mathf.Max(1, savedOrder.quantity),
                savedOrder.rewardGold,
                savedOrder.rewardExp,
                savedOrder.rewardInfluence,
                savedOrder.rewardSatisfaction
            );

            restoredCount++;
        }

        Debug.Log($"[세이브] 주문 불러오기 완료: {restoredCount}개 복원");
    }
}