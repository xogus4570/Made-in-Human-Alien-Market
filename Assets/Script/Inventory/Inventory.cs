using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;
    public event System.Action Changed;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log($"[Inventory] KEEP: {name}");
        }
        else if (instance != this)
        {
            Debug.Log($"[Inventory] DESTROY DUP: {name}");
            Destroy(gameObject);
            return;
        }
    }

    [System.Serializable]
    public class Slot { public Item item; public int count; }

    private readonly List<Slot> slots = new List<Slot>();



    // ID로 비교
    private static bool SameItem(Item a, Item b) =>
        a != null && b != null && !string.IsNullOrEmpty(a.id) && a.id == b.id;

    public void Add(Item item, int count = 1)
    {
        if (item == null) return;
        var slot = slots.Find(s => SameItem(s.item, item));
        if (slot != null) slot.count += count;
        else slots.Add(new Slot { item = item, count = count });

        Changed?.Invoke();
    }

    public bool Remove(Item item, int count = 1)
    {
        if (item == null || count <= 0) return false;
        var slot = slots.Find(s => SameItem(s.item, item));
        if (slot == null || slot.count < count) return false;
        slot.count -= count;
        if (slot.count <= 0) slots.Remove(slot);
        
        Changed?.Invoke();              
        return true;
    }

    public int GetCount(Item item)
    {
        if (item == null) return 0;
        var slot = slots.Find(s => SameItem(s.item, item));
        return slot != null ? slot.count : 0;
    }

    // 외부에서 리스트를 변경 못하게 보호
    public IReadOnlyList<Slot> GetAllSlots() => slots.AsReadOnly();
}
