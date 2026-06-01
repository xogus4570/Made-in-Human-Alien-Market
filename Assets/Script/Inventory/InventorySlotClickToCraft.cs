using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotClickToCraft : MonoBehaviour, IPointerClickHandler
{
    public InventorySlotUI slot;

    [Header("컨트롤러")]
    public CraftingController craftingController;
    public PrintController printController;
    public PackingController packingController;
    public PackagingController packagingController;

    private void Awake()
    {
        if (slot == null)
            slot = GetComponent<InventorySlotUI>();

        if (craftingController == null)
            craftingController = FindFirstObjectByType<CraftingController>(FindObjectsInactive.Include);

        if (printController == null)
            printController = FindFirstObjectByType<PrintController>(FindObjectsInactive.Include);

        if (packingController == null)
            packingController = FindFirstObjectByType<PackingController>(FindObjectsInactive.Include);

        if (packagingController == null)
            packagingController = FindFirstObjectByType<PackagingController>(FindObjectsInactive.Include);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (slot == null || slot.item == null || slot.count <= 0)
            return;

        if (packagingController != null && packagingController.gameObject.activeInHierarchy)
        {
            packagingController.OnClickInventoryItem(slot.item);
            return;
        }

        if (craftingController != null && craftingController.gameObject.activeInHierarchy)
        {
            craftingController.OnClickInventoryItem(slot.item);
            return;
        }

        if (printController != null && printController.gameObject.activeInHierarchy)
        {
            printController.OnClickInventoryItem(slot.item);
            return;
        }

        if (packingController != null && packingController.gameObject.activeInHierarchy)
        {
            packingController.OnClickInventoryItem(slot.item);
            return;
        }

        Debug.LogWarning("[InventorySlotClickToCraft] 활성화된 컨트롤러 없음");
    }
}