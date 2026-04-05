using UnityEngine;

public class DeliveryInventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject deliveryRoot;
    [SerializeField] private GameObject deliveryInventoryPanel;
    [SerializeField] private Transform gridRoot;

    private InventorySlotUI[] uiSlots;

    private void Awake()
    {
        if (gridRoot == null)
            gridRoot = transform;

        uiSlots = gridRoot.GetComponentsInChildren<InventorySlotUI>(true);
    }

    private void Start()
    {
        if (deliveryRoot != null)
            deliveryRoot.SetActive(false);

        if (Inventory.instance != null)
            Inventory.instance.Changed += Refresh;

        Refresh();
    }

    private void OnDestroy()
    {
        if (Inventory.instance != null)
            Inventory.instance.Changed -= Refresh;
    }

    public void Open()
    {
        if (deliveryRoot != null)
            deliveryRoot.SetActive(true);

        Refresh();
    }

    public void Close()
    {
        if (deliveryRoot != null)
            deliveryRoot.SetActive(false);
    }

    public bool IsOpen()
    {
        return deliveryRoot != null && deliveryRoot.activeSelf;
    }

    public void Refresh()
    {
        if (uiSlots == null) return;
        if (Inventory.instance == null) return;

        foreach (var slot in uiSlots)
            slot.Clear();

        var slots = Inventory.instance.GetAllSlots();
        int count = Mathf.Min(uiSlots.Length, slots.Count);

        for (int i = 0; i < count; i++)
            uiSlots[i].SetItem(slots[i].item, slots[i].count);
    }
}