using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PackagingController : MonoBehaviour
{
    [SerializeField] private Image slotIcon;
    [SerializeField] private TMP_Text countText;

    private Item selectedItem;

    public void OnClickInventoryItem(Item item)
    {
        if (item == null) return;

        if (item.itemType != ItemType.Goods)
        {
            Debug.Log("[Packaging] 굿즈만 포장 가능");
            return;
        }

        selectedItem = item;

        if (slotIcon != null)
        {
            slotIcon.sprite = item.itemImage;
            slotIcon.enabled = item.itemImage != null;
        }

        if (countText != null)
            countText.text = "1";

        Debug.Log($"[Packaging] 슬롯에 넣음: {item.itemName}");
    }

    public void PackButton()
    {
        if (selectedItem == null)
        {
            Debug.LogWarning("[Packaging] 포장할 굿즈 없음");
            return;
        }

        if (Inventory.instance == null)
        {
            Debug.LogWarning("[Packaging] Inventory 없음");
            return;
        }

        if (PackedGoodsManager.Instance == null)
        {
            Debug.LogWarning("[Packaging] PackedGoodsManager 없음");
            return;
        }

        if (Inventory.instance.GetCount(selectedItem) <= 0)
        {
            Debug.LogWarning("[Packaging] 인벤토리에 해당 굿즈 없음");
            return;
        }

        PackedGoodsManager.Instance.AddPacked(selectedItem, 1);

        Debug.Log($"[Packaging] 포장 등록 완료: {selectedItem.itemName}");

        ClearSlot();
    }

    private void ClearSlot()
    {
        selectedItem = null;

        if (slotIcon != null)
        {
            slotIcon.sprite = null;
            slotIcon.enabled = false;
        }

        if (countText != null)
            countText.text = "";
    }
}