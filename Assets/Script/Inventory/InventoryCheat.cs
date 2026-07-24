using UnityEngine;

public class InventoryCheat : MonoBehaviour
{
    [Header("치트 키")]
    [SerializeField] private KeyCode cheatKey = KeyCode.C;

    [Header("지급 개수")]
    [SerializeField] private int addCount = 1;

    [Header("로그 출력")]
    [SerializeField] private bool showLog = true;

    private void Update()
    {
        if (Input.GetKeyDown(cheatKey))
        {
            AddAllMaterialsAndGems();
        }
    }

    private void AddAllMaterialsAndGems()
    {
        if (Inventory.instance == null)
        {
            Debug.LogWarning("[인벤토리 치트] 실패: Inventory.instance가 없습니다.");
            return;
        }

        if (ItemDataBase.instance == null)
        {
            Debug.LogWarning("[인벤토리 치트] 실패: ItemDataBase.instance가 없습니다.");
            return;
        }

        if (ItemDataBase.instance.itemDB == null || ItemDataBase.instance.itemDB.Count == 0)
        {
            Debug.LogWarning("[인벤토리 치트] 실패: ItemDB가 비어 있습니다.");
            return;
        }

        int safeAddCount = Mathf.Max(1, addCount);
        int addedKindCount = 0;
        int addedTotalCount = 0;

        foreach (Item item in ItemDataBase.instance.itemDB)
        {
            if (item == null)
                continue;

            if (string.IsNullOrEmpty(item.id))
                continue;

            if (item.itemType != ItemType.Material && item.itemType != ItemType.Gem)
                continue;

            Inventory.instance.Add(item, safeAddCount);

            addedKindCount++;
            addedTotalCount += safeAddCount;

            if (showLog)
                Debug.Log($"[인벤토리 치트] 지급: {item.itemName} x{safeAddCount}");
        }

        if (addedKindCount <= 0)
        {
            Debug.LogWarning("[인벤토리 치트] 지급 실패: ItemDB 안에 Material 또는 Gem 타입 아이템이 없습니다.");
            return;
        }

        Debug.Log($"[인벤토리 치트] 완료: 재료/보석 {addedKindCount}종, 총 {addedTotalCount}개 지급");
    }
}