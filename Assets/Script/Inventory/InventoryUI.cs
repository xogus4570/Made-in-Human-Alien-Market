using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    private enum InventoryTab
    {
        Material,
        Goods
    }

    [Header("Slot Root")]
    [SerializeField] private Transform gridRoot;

    [Header("Background")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Sprite materialBackground;
    [SerializeField] private Sprite goodsBackground;

    [Header("Inventory Root")]
    [SerializeField] private GameObject inventoryRoot;

    private InventorySlotUI[] uiSlots;

    private bool activeInventory = false;

    private InventoryTab currentTab = InventoryTab.Material;

    private void Awake()
    {
        if (gridRoot == null)
            gridRoot = transform;

        uiSlots = gridRoot.GetComponentsInChildren<InventorySlotUI>(true);
    }

    private void Start()
    {
        if (inventoryRoot != null)
            inventoryRoot.SetActive(false);

        if (backgroundImage != null && materialBackground != null)
            backgroundImage.sprite = materialBackground;

        if (Inventory.instance != null)
            Inventory.instance.Changed += Refresh;

        Refresh();
    }

    private void OnDestroy()
    {
        if (Inventory.instance != null)
            Inventory.instance.Changed -= Refresh;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            activeInventory = !activeInventory;

            if (inventoryRoot != null)
                inventoryRoot.SetActive(activeInventory);

            if (activeInventory)
                Refresh();
        }
    }

    public void ShowMaterialTab()
    {
        currentTab = InventoryTab.Material;

        if (backgroundImage != null && materialBackground != null)
            backgroundImage.sprite = materialBackground;

        Refresh();
    }

    public void ShowGoodsTab()
    {
        currentTab = InventoryTab.Goods;

        if (backgroundImage != null && goodsBackground != null)
            backgroundImage.sprite = goodsBackground;

        Refresh();
    }

    public void CloseInventory()
    {
        activeInventory = false;

        if (inventoryRoot != null)
            inventoryRoot.SetActive(false);
    }

    private void Refresh()
    {
        if (uiSlots == null)
            return;

        foreach (var slot in uiSlots)
            slot.Clear();

        if (Inventory.instance == null)
            return;

        var slots = Inventory.instance.GetAllSlots();

        int index = 0;

        foreach (var slot in slots)
        {
            if (slot.item == null)
                continue;

            if (!IsVisibleInCurrentTab(slot.item))
                continue;

            if (index >= uiSlots.Length)
                break;

            uiSlots[index].SetItem(slot.item, slot.count);

            index++;
        }
    }

    private bool IsVisibleInCurrentTab(Item item)
    {
        if (item == null)
            return false;

        if (currentTab == InventoryTab.Material)
        {
            return item.itemType == ItemType.Material
                || item.itemType == ItemType.Gem;
        }

        if (currentTab == InventoryTab.Goods)
        {
            return item.itemType == ItemType.Goods;
        }

        return false;
    }
}