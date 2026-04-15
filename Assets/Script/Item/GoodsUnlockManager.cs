using System.Collections.Generic;
using UnityEngine;

public class GoodsUnlockManager : MonoBehaviour
{
    public static GoodsUnlockManager Instance;

    private readonly HashSet<string> unlockedGoodsIds = new HashSet<string>();

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
        }
    }

    public bool IsUnlocked(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
            return false;

        return unlockedGoodsIds.Contains(itemId);
    }

    public bool Unlock(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("[GoodsUnlockManager] itemId가 비어 있습니다.");
            return false;
        }

        if (unlockedGoodsIds.Contains(itemId))
        {
            Debug.Log($"[GoodsUnlockManager] 이미 해금된 굿즈입니다: {itemId}");
            return false;
        }

        unlockedGoodsIds.Add(itemId);
        Debug.Log($"[GoodsUnlockManager] 굿즈 해금 완료: {itemId}");
        return true;
    }

    public IReadOnlyCollection<string> GetUnlockedGoodsIds()
    {
        return unlockedGoodsIds;
    }
}