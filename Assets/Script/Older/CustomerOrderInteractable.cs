using System.Collections.Generic;
using UnityEngine;

public class CustomerOrderInteractable : MonoBehaviour, IInteractable
{
    [Header("대화 UI")]
    [SerializeField] private OrderDialogUI dialogUI;

    [Header("레시피 DB")]
    [SerializeField] private RecipeDB recipeDB;

    [Header("손님 목록")]
    [SerializeField]
    private List<CustomerOrderProfile> customers =
        new List<CustomerOrderProfile>();

    public string GetInteractionName()
    {
        return "주문 받기";
    }

    public void OnInteract(GameObject interactor)
    {
        Debug.Log("[CustomerOrderInteractable] 상호작용 시작");

        if (customers == null || customers.Count <= 0)
        {
            Debug.LogWarning("[CustomerOrderInteractable] 손님이 없습니다.");
            return;
        }

        CustomerOrderProfile customer =
            customers[Random.Range(0, customers.Count)];

        bool created = CreateOrder(customer);

        Debug.Log("[CustomerOrderInteractable] 주문 생성 결과: " + created);

        if (created && dialogUI != null)
        {
            Debug.Log("[CustomerOrderInteractable] 대화창 열기 시도");

            dialogUI.Open(
                customer.customerName,
                customer.customerSprite,
                customer.dialogMessage
            );
        }
        else
        {
            Debug.LogWarning("[CustomerOrderInteractable] dialogUI가 없거나 주문 생성 실패");
        }
    }

    private bool CreateOrder(CustomerOrderProfile customer)
    {
        if (recipeDB == null) return false;
        if (ItemDataBase.instance == null) return false;
        if (OrderManager.Instance == null) return false;

        List<Recipe> candidates = new List<Recipe>();

        foreach (string resultId in customer.orderableResultIds)
        {
            if (string.IsNullOrEmpty(resultId))
                continue;

            // 해금 안된 굿즈 제외
            if (GoodsUnlockManager.Instance != null)
            {
                if (!GoodsUnlockManager.Instance.IsUnlocked(resultId))
                    continue;
            }

            Recipe recipe = recipeDB.FindByResultId(resultId);

            if (recipe == null)
                continue;

            Item item =
                ItemDataBase.instance.GetById(resultId);

            if (item == null)
                continue;

            candidates.Add(recipe);
        }

        if (candidates.Count <= 0)
        {
            Debug.LogWarning("[CustomerOrderInteractable] 주문 후보 없음");
            return false;
        }

        Recipe selectedRecipe =
            candidates[Random.Range(0, candidates.Count)];

        Item selectedItem =
            ItemDataBase.instance.GetById(selectedRecipe.resultId);

        if (selectedItem == null)
            return false;

        OrderManager.Instance.AddOrder(
            selectedItem,
            1,
            selectedRecipe.rewardGold,
            selectedRecipe.rewardExp,
            selectedRecipe.rewardInfluence,
            selectedRecipe.rewardSatisfaction
        );

        Debug.Log($"[Order] 생성: {selectedItem.itemName}");

        return true;
    }

}