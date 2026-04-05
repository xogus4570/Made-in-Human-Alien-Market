using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotClickToDeliver : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private InventorySlotUI slot;

    private void Awake()
    {
        if (slot == null)
            slot = GetComponent<InventorySlotUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot == null) return;
        if (slot.item == null || slot.count <= 0) return;
        if (DeliveryStorageUI.Instance == null) return;

        Item clickedItem = slot.item;

        bool removed = Inventory.instance.Remove(clickedItem, 1);
        if (!removed) return;

        bool added = DeliveryStorageUI.Instance.AddItemToStorage(clickedItem, 1);
        if (!added)
        {
            Inventory.instance.Add(clickedItem, 1);
            Debug.Log("[Delivery] 창고 추가 실패");
            return;
        }

        Debug.Log($"[Delivery] 창고로 이동: {clickedItem.itemName}");
    }
}