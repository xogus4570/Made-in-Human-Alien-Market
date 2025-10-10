using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotClickToCraft : MonoBehaviour, IPointerClickHandler
{
    public InventorySlotUI slot;
    public CraftingController controller;

    void Awake()
    {
        if (slot == null) slot = GetComponent<InventorySlotUI>();
        if (controller == null) controller = FindObjectOfType<CraftingController>(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (controller == null || slot == null) return;
        if (slot.item == null || slot.count <= 0) return;

        controller.OnClickInventoryItem(slot.item);
    }
}
