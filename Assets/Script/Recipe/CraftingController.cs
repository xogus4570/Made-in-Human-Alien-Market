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

    [Header("미니게임")]
    [SerializeField] private CraftingMinigameController minigameController;

    private void Start()
    {
        craftButton.onClick.AddListener(Craft);
        clearButton.onClick.AddListener(ClearAll);
        RefreshButtons();
    }

    public void OnClickInventoryItem(Item clickedItem)
    {
        if (clickedItem == null) return;

        if (clickedItem.itemType != ItemType.Material &&
            clickedItem.itemType != ItemType.Gem)
            return;

        if (slotA.IsEmpty) TryPut(slotA, clickedItem);
        else if (slotB.IsEmpty) TryPut(slotB, clickedItem);
        else if (slotC.IsEmpty) TryPut(slotC, clickedItem);

        RefreshButtons();
    }

    private void TryPut(IngredientSlotUI slot, Item item)
    {
        if (Inventory.instance == null) return;
        if (Inventory.instance.GetCount(item) <= 0) return;

        slot.Set(item, 1);
    }

    private void RefreshButtons()
    {
        if (craftButton != null)
            craftButton.interactable = HasValidRecipe();
    }

    private bool HasValidRecipe()
    {
        if (slotA.IsEmpty || slotB.IsEmpty || slotC.IsEmpty) return false;
        if (recipeDB == null) return false;

        Recipe recipe = recipeDB.Find(slotA.item.id, slotB.item.id, slotC.item.id);
        if (recipe == null) return false;

        Item resultItem = ItemDataBase.instance.GetById(recipe.resultId);
        if (resultItem == null) return false;

        if (resultItem.itemType == ItemType.Goods)
        {
            if (GoodsUnlockManager.Instance == null) return false;
            if (!GoodsUnlockManager.Instance.IsUnlocked(resultItem.id)) return false;
        }

        return true;
    }

    public void Craft()
    {
        if (!HasValidRecipe()) return;

        Recipe recipe = recipeDB.Find(slotA.item.id, slotB.item.id, slotC.item.id);

        if (recipe == null) return;
        if (!CanConsume(slotA) || !CanConsume(slotB) || !CanConsume(slotC)) return;

        if (minigameController == null)
        {
            Debug.LogWarning("[CraftingController] 미니게임 컨트롤러가 연결되지 않았습니다.");
            return;
        }

        minigameController.StartMinigame(slotA, slotB, slotC, recipe);
    }

    private bool CanConsume(IngredientSlotUI slot)
    {
        if (slot == null || slot.item == null) return false;
        if (Inventory.instance == null) return false;

        return Inventory.instance.GetCount(slot.item) >= slot.count;
    }

    public void ClearAll()
    {
        if (slotA != null) slotA.Clear();
        if (slotB != null) slotB.Clear();
        if (slotC != null) slotC.Clear();

        RefreshButtons();
    }
}