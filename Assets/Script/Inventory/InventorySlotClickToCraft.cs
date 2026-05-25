using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotClickToCraft : MonoBehaviour, IPointerClickHandler
{
    public InventorySlotUI slot;

    [Header("컨트롤러")]
    public CraftingController craftingController;
    public PrintController printController;
    public PackingController packingController;

    void Awake()
    {
        if (slot == null)
            slot = GetComponent<InventorySlotUI>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot == null) return;
        if (slot.item == null || slot.count <= 0) return;

        if (craftingController != null)
        {
            craftingController.OnClickInventoryItem(slot.item);
            return;
        }

        if (printController != null)
        {
            printController.OnClickInventoryItem(slot.item);
            return;
        }

        if (packingController != null)
        {
            packingController.OnClickInventoryItem(slot.item);
            return;
        }

        Debug.LogWarning("[InventorySlotClickToCraft] 연결된 컨트롤러가 없습니다.");
    }
}