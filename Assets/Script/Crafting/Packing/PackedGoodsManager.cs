using System.Collections.Generic;
using UnityEngine;

public class PackedGoodsManager : MonoBehaviour
{
    public static PackedGoodsManager Instance { get; private set; }

    private readonly Dictionary<string, int> packedCounts = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddPacked(Item item, int count = 1)
    {
        if (item == null || string.IsNullOrEmpty(item.id) || count <= 0)
            return;

        if (!packedCounts.ContainsKey(item.id))
            packedCounts[item.id] = 0;

        packedCounts[item.id] += count;

        Debug.Log($"[PackedGoodsManager] 포장 등록: {item.itemName} / 현재 포장 수량: {packedCounts[item.id]}");
    }

    public bool HasPacked(Item item)
    {
        if (item == null || string.IsNullOrEmpty(item.id))
            return false;

        return packedCounts.ContainsKey(item.id) && packedCounts[item.id] > 0;
    }

    public bool UsePacked(Item item)
    {
        if (!HasPacked(item))
            return false;

        packedCounts[item.id]--;

        if (packedCounts[item.id] <= 0)
            packedCounts.Remove(item.id);

        Debug.Log($"[PackedGoodsManager] 포장 사용: {item.itemName}");

        return true;
    }

    public int GetPackedCount(Item item)
    {
        if (item == null || string.IsNullOrEmpty(item.id))
            return 0;

        return packedCounts.TryGetValue(item.id, out int count) ? count : 0;
    }
}