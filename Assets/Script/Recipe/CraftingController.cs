using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingController : MonoBehaviour
{
    [Header("Refs")]
    public IngredientSlotUI slotA;
    public IngredientSlotUI slotB;
    public IngredientSlotUI slotC;
    public RecipeDB recipeDB;
    public Button craftButton;
    public Button clearButton;

    private void Start()
    {
        craftButton.onClick.AddListener(Craft);
        clearButton.onClick.AddListener(ClearAll);
        RefreshButtons();
    }

    public void OnClickInventoryItem(Item clickedItem)
    {
        // (간단) 재료 타입만 허용하고 싶으면 여기서 필터링:
        // if (clickedItem.itemType != ItemType.Material) return;

        // 빈 슬롯에 채우기
        if (slotA.IsEmpty) { TryPut(slotA, clickedItem); }
        else if (slotB.IsEmpty) { TryPut(slotB, clickedItem); }
        else if (slotC.IsEmpty) { TryPut(slotC, clickedItem); }
        RefreshButtons();
    }

    void TryPut(IngredientSlotUI slot, Item it)
    {
        // 인벤토리에 있는지 확인(최소 1개)
        if (Inventory.instance.GetCount(it) <= 0) return;
        slot.Set(it, 1); // 우선 1개만 사용(필요시 +/– 버튼 추가)
    }

    void RefreshButtons()
    {
        craftButton.interactable = HasValidRecipe();
    }

    bool HasValidRecipe()
    {
        if (slotA.IsEmpty || slotB.IsEmpty || slotC.IsEmpty) return false;
        var r = recipeDB.Find(slotA.item.id, slotB.item.id, slotC.item.id);
        return r != null;
    }

    void Craft()
    {
        if (!HasValidRecipe()) return;
        var r = recipeDB.Find(slotA.item.id, slotB.item.id, slotC.item.id);
        // 재료 차감 가능 여부 확인
        if (!CanConsume(slotA) || !CanConsume(slotB) || !CanConsume(slotC)) return;

        // 차감
        Inventory.instance.Remove(slotA.item, slotA.count);
        Inventory.instance.Remove(slotB.item, slotB.count);
        Inventory.instance.Remove(slotC.item, slotC.count);

        // 결과 지급
        var outItem = ItemDataBase.instance.GetById(r.resultId);
        if (outItem != null)
            Inventory.instance.Add(outItem, Mathf.Max(1, r.resultCount));

        ClearAll();
        RefreshButtons();
        // 필요하면 여기서 제작 성공 연출/사운드
    }

    bool CanConsume(IngredientSlotUI s)
    {
        return s.item != null && Inventory.instance.GetCount(s.item) >= s.count;
    }

    public void ClearAll()
    {
        slotA.Clear(); slotB.Clear(); slotC.Clear();
    }
}
