using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OrderCardUI : MonoBehaviour
{
    [SerializeField] private Image productIcon;
    [SerializeField] private TextMeshProUGUI quantityText;

    [SerializeField] private Image ingredientAIcon;
    [SerializeField] private Image ingredientBIcon;
    [SerializeField] private Image ingredientCIcon;

    [SerializeField] private RecipeDB recipeDB;

    [SerializeField] private Image orderPanelImage;

    [Header("보석별 주문 카드 배경")]
    [SerializeField] private Sprite aquaPanelSprite;
    [SerializeField] private Sprite citronPanelSprite;
    [SerializeField] private Sprite blackPearlPanelSprite;
    [SerializeField] private Sprite rubyPanelSprite;

    private void ApplyOrderPanelByItemId(string itemId)
    {
        if (orderPanelImage == null || string.IsNullOrEmpty(itemId))
            return;

        if (itemId.EndsWith("_A"))
            orderPanelImage.sprite = aquaPanelSprite;
        else if (itemId.EndsWith("_C"))
            orderPanelImage.sprite = citronPanelSprite;
        else if (itemId.EndsWith("_B"))
            orderPanelImage.sprite = blackPearlPanelSprite;
        else if (itemId.EndsWith("_R"))
            orderPanelImage.sprite = rubyPanelSprite;
    }

    public void SetRecipeDB(RecipeDB db)
    {
        recipeDB = db;
    }

    public void SetOrder(OrderData order)
    {
        if (order == null || order.orderedItem == null) return;

        if (productIcon != null)
        {
            productIcon.sprite = order.orderedItem.itemImage;
            productIcon.enabled = order.orderedItem.itemImage != null;
        }

        if (quantityText != null)
        {
            quantityText.text = "x" + order.quantity;
        }

        if (recipeDB == null)
        {
            ClearIngredientIcons();
            return;
        }

        Recipe recipe = recipeDB.FindByResultId(order.orderedItem.id);
        if (recipe == null)
        {
            ClearIngredientIcons();
            return;
        }

        SetIngredient(ingredientAIcon, recipe.ingredientA);
        SetIngredient(ingredientBIcon, recipe.ingredientB);
        SetIngredient(ingredientCIcon, recipe.ingredientC);
        ApplyOrderPanelByItemId(order.orderedItem.id);
    }

    private void SetIngredient(Image icon, string itemId)
    {
        if (icon == null || ItemDataBase.instance == null) return;

        var item = ItemDataBase.instance.GetById(itemId);
        if (item == null)
        {
            icon.sprite = null;
            icon.enabled = false;
            return;
        }

        icon.sprite = item.itemImage;
        icon.enabled = item.itemImage != null;
    }

    private void ClearIngredientIcons()
    {
        ClearImage(ingredientAIcon);
        ClearImage(ingredientBIcon);
        ClearImage(ingredientCIcon);
    }

    private void ClearImage(Image icon)
    {
        if (icon == null) return;
        icon.sprite = null;
        icon.enabled = false;
    }

}

