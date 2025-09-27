using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform gridRoot; // 슬롯들이 들어있는 부모
    private InventorySlotUI[] uiSlots;
    bool activeInventory = false;

    private void Awake()
    {
        if (gridRoot == null) gridRoot = transform;
        uiSlots = gridRoot.GetComponentsInChildren<InventorySlotUI>(true);
    }

    private void Start()
    {
        inventoryPanel.SetActive(activeInventory);

        //인벤토리 변경 이벤트 구독
        if (Inventory.instance != null)
            Inventory.instance.Changed += Refresh;

        Refresh(); // 초기 1회 실행
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
            inventoryPanel.SetActive(activeInventory);

            if (activeInventory) Refresh();
        }
    }

    //슬롯 UI 새로고침
    private void Refresh()
    {
        // 먼저 다 비우기
        foreach (var slot in uiSlots) slot.Clear();

        // 인벤토리 데이터 가져오기
        var slots = Inventory.instance.GetAllSlots();
        int count = Mathf.Min(uiSlots.Length, slots.Count);

        for (int i = 0; i < count; i++)
            uiSlots[i].SetItem(slots[i].item, slots[i].count);
    }
}
