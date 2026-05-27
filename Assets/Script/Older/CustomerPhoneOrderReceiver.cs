using System.Collections.Generic;
using UnityEngine;

public class CustomerPhoneOrderReceiver : MonoBehaviour, IInteractable
{
    [Header("대화 UI")]
    [SerializeField] private OrderDialogUI dialogUI;

    [Header("랜덤 주문용 레시피 DB")]
    [SerializeField] private RecipeDB recipeDB;

    [Header("손님 목록")]
    [SerializeField] private List<CustomerOrderProfile> customers = new List<CustomerOrderProfile>();

    [Header("주문 수량 범위")]
    [SerializeField] private int minOrderQuantity = 1;
    [SerializeField] private int maxOrderQuantity = 1;

    [Header("주문 제한시간")]
    [SerializeField] private float incomingOrderDuration = 10f;

    [Header("실패 패널티")]
    [SerializeField] private GameStatusUI gameStatusUI;
    [SerializeField] private int missedOrderSatisfactionPenalty = 5;

    private bool hasIncomingOrder = false;
    private float remainingTime = 0f;

    private CustomerOrderProfile pendingCustomer;
    private Item pendingItem;
    private int pendingQuantity;

    private int pendingRewardGold;
    private int pendingRewardExp;
    private int pendingRewardInfluence;
    private int pendingRewardSatisfaction;

    public bool HasIncomingOrder => hasIncomingOrder;

    private void Update()
    {
        if (!hasIncomingOrder)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
            ExpireIncomingOrder();
    }

    public string GetInteractionName()
    {
        return hasIncomingOrder ? "주문 받기" : "주문 확인";
    }

    public void OnInteract(GameObject interactor)
    {
        if (!hasIncomingOrder)
        {
            Debug.Log("[CustomerPhoneOrderReceiver] 현재 들어온 주문이 없습니다.");
            return;
        }

        AcceptOrder();
    }

    public void CreateTestIncomingOrder()
    {
        if (hasIncomingOrder)
        {
            Debug.Log("[CustomerPhoneOrderReceiver] 이미 들어온 주문이 있습니다.");
            return;
        }

        if (!PrepareIncomingOrder())
            return;

        hasIncomingOrder = true;
        remainingTime = incomingOrderDuration;

        Debug.Log($"[CustomerPhoneOrderReceiver] 주문 도착: {pendingCustomer.customerName} / {pendingItem.itemName} x{pendingQuantity}");
    }

    private bool PrepareIncomingOrder()
    {
        if (recipeDB == null)
        {
            Debug.LogError("[CustomerPhoneOrderReceiver] RecipeDB가 연결되지 않았습니다.");
            return false;
        }

        if (ItemDataBase.instance == null)
        {
            Debug.LogError("[CustomerPhoneOrderReceiver] ItemDataBase.instance가 없습니다.");
            return false;
        }

        if (customers == null || customers.Count == 0)
        {
            Debug.LogError("[CustomerPhoneOrderReceiver] 손님 목록이 비어 있습니다.");
            return false;
        }

        List<CustomerOrderCandidate> candidates = new List<CustomerOrderCandidate>();

        foreach (CustomerOrderProfile customer in customers)
        {
            if (customer == null || customer.orderableResultIds == null)
                continue;

            foreach (string resultId in customer.orderableResultIds)
            {
                if (string.IsNullOrEmpty(resultId))
                    continue;

                if (GoodsUnlockManager.Instance != null &&
                    !GoodsUnlockManager.Instance.IsUnlocked(resultId))
                    continue;

                Recipe recipe = recipeDB.FindByResultId(resultId);
                if (recipe == null)
                    continue;

                Item item = ItemDataBase.instance.GetById(resultId);
                if (item == null)
                    continue;

                candidates.Add(new CustomerOrderCandidate(customer, recipe, item));
            }
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning("[CustomerPhoneOrderReceiver] 생성 가능한 주문 후보가 없습니다.");
            return false;
        }

        CustomerOrderCandidate selected = candidates[Random.Range(0, candidates.Count)];

        pendingCustomer = selected.customer;
        pendingItem = selected.item;

        pendingQuantity = Random.Range(
            Mathf.Max(1, minOrderQuantity),
            Mathf.Max(1, maxOrderQuantity) + 1
        );

        pendingRewardGold = selected.recipe.rewardGold;
        pendingRewardExp = selected.recipe.rewardExp;
        pendingRewardInfluence = selected.recipe.rewardInfluence;
        pendingRewardSatisfaction = selected.recipe.rewardSatisfaction;

        return true;
    }

    private void AcceptOrder()
    {
        if (OrderManager.Instance == null)
        {
            Debug.LogError("[CustomerPhoneOrderReceiver] OrderManager.Instance가 없습니다.");
            return;
        }

        if (pendingItem == null || pendingCustomer == null)
        {
            Debug.LogWarning("[CustomerPhoneOrderReceiver] 대기 중인 주문 정보가 없습니다.");
            ClearIncomingOrder();
            return;
        }

        OrderManager.Instance.AddOrder(
            pendingItem,
            pendingQuantity,
            pendingRewardGold,
            pendingRewardExp,
            pendingRewardInfluence,
            pendingRewardSatisfaction
        );

        if (dialogUI != null)
        {
            dialogUI.Open(
                pendingCustomer.customerName,
                pendingCustomer.customerSprite,
                pendingCustomer.dialogMessage
            );
        }

        Debug.Log($"[CustomerPhoneOrderReceiver] 주문 수락 완료: {pendingItem.itemName} x{pendingQuantity}");

        ClearIncomingOrder();
    }

    private void ExpireIncomingOrder()
    {
        Debug.Log("[CustomerPhoneOrderReceiver] 주문 시간이 만료되었습니다.");

        if (gameStatusUI != null)
        {
            gameStatusUI.ReduceSatisfaction(missedOrderSatisfactionPenalty);
            Debug.Log($"[CustomerPhoneOrderReceiver] 고객만족도 {missedOrderSatisfactionPenalty} 감소");
        }
        else
        {
            Debug.LogWarning("[CustomerPhoneOrderReceiver] GameStatusUI가 연결되지 않았습니다.");
        }

        ClearIncomingOrder();
    }

    private void ClearIncomingOrder()
    {
        hasIncomingOrder = false;
        remainingTime = 0f;

        pendingCustomer = null;
        pendingItem = null;
        pendingQuantity = 0;

        pendingRewardGold = 0;
        pendingRewardExp = 0;
        pendingRewardInfluence = 0;
        pendingRewardSatisfaction = 0;
    }

    private class CustomerOrderCandidate
    {
        public CustomerOrderProfile customer;
        public Recipe recipe;
        public Item item;

        public CustomerOrderCandidate(CustomerOrderProfile customer, Recipe recipe, Item item)
        {
            this.customer = customer;
            this.recipe = recipe;
            this.item = item;
        }
    }
}