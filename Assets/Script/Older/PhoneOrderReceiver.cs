using System.Collections.Generic;
using UnityEngine;

public class PhoneOrderReceiver : MonoBehaviour, IInteractable
{
    [Header("전화 주문용 레시피 DB")]
    [SerializeField] private RecipeDB recipeDB;

    [Header("�주문 수량 범위")]
    [SerializeField] private int minOrderQuantity = 1;
    [SerializeField] private int maxOrderQuantity = 1;

    [Header("주문 제한시간")]
    [SerializeField] private float incomingOrderDuration = 10f;

    [Header("만족도 패널티")]
    [SerializeField] private GameStatusUI gameStatusUI;
    [SerializeField] private int missedOrderSatisfactionPenalty = 5;

    private bool hasIncomingOrder = false;
    private Item pendingItem;
    private int pendingQuantity;
    private float remainingTime = 0f;

    public bool HasIncomingOrder => hasIncomingOrder;

    private int pendingRewardGold;
    private int pendingRewardExp;
    private int pendingRewardInfluence;
    private int pendingRewardSatisfaction;

    private void Update()
    {
        if (!hasIncomingOrder) return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            ExpireIncomingOrder();
        }
    }

    public string GetInteractionName()
    {
        return hasIncomingOrder ? "전화 받기" : "전화기 확인";
    }

    public void OnInteract(GameObject interactor)
    {
        if (!hasIncomingOrder)
        {
            Debug.Log("[PhoneOrderReceiver] 현재 들어온 주문이 없습니다.");
            return;
        }

        AcceptOrder();
    }

    public void CreateTestIncomingOrder()
    {
        if (hasIncomingOrder)
        {
            Debug.Log("[PhoneOrderReceiver] 이미 들어온 주문이 있습니다.");
            return;
        }

        if (recipeDB == null)
        {
            Debug.LogError("[PhoneOrderReceiver] RecipeDB가 연결되지 않았습니다.");
            return;
        }

        if (ItemDataBase.instance == null)
        {
            Debug.LogError("[PhoneOrderReceiver] ItemDataBase.instance 가 없습니다.");
            return;
        }

        if (GoodsUnlockManager.Instance == null)
        {
            Debug.LogError("[PhoneOrderReceiver] GoodsUnlockManager.Instance 가 없습니다.");
            return;
        }

        if (recipeDB.recipes == null || recipeDB.recipes.Count == 0)
        {
            Debug.LogError("[PhoneOrderReceiver] RecipeDB에 등록된 레시피가 없습니다.");
            return;
        }

        List<Recipe> unlockedRecipes = new List<Recipe>();

        for (int i = 0; i < recipeDB.recipes.Count; i++)
        {
            Recipe candidate = recipeDB.recipes[i];

            if (candidate == null) continue;
            if (string.IsNullOrEmpty(candidate.resultId)) continue;

            if (!GoodsUnlockManager.Instance.IsUnlocked(candidate.resultId))
                continue;

            Item resultItem = ItemDataBase.instance.GetById(candidate.resultId);
            if (resultItem == null) continue;

            if (resultItem.itemType != ItemType.Goods)
                continue;

            unlockedRecipes.Add(candidate);
        }

        if (unlockedRecipes.Count == 0)
        {
            Debug.LogWarning("[PhoneOrderReceiver] 주문 생성 실패: 해금된 유효 굿즈 레시피가 없습니다.");
            return;
        }

        int randomIndex = Random.Range(0, unlockedRecipes.Count);
        Recipe selectedRecipe = unlockedRecipes[randomIndex];

        Item item = ItemDataBase.instance.GetById(selectedRecipe.resultId);
        if (item == null)
        {
            Debug.LogError($"[PhoneOrderReceiver] resultId '{selectedRecipe.resultId}'신규 주문 생성: {{pendingItem.itemName}} x{{pendingQuantity}}\"");
            return;
        }

        hasIncomingOrder = true;
        pendingItem = item;
        pendingQuantity = Random.Range(
            Mathf.Max(1, minOrderQuantity),
            Mathf.Max(1, maxOrderQuantity) + 1
        );
        remainingTime = incomingOrderDuration;

        pendingRewardGold = selectedRecipe.rewardGold;
        pendingRewardExp = selectedRecipe.rewardExp;
        pendingRewardInfluence = selectedRecipe.rewardInfluence;
        pendingRewardSatisfaction = selectedRecipe.rewardSatisfaction;

        if (selectedRecipe.rewardGold == 0 &&
            selectedRecipe.rewardExp == 0 &&
            selectedRecipe.rewardInfluence == 0 &&
            selectedRecipe.rewardSatisfaction == 0)
        {
            Debug.LogWarning($"[Recipe] ���� ���� �ȵ�: {selectedRecipe.resultId}");
        }

        Debug.Log($"[PhoneOrderReceiver] ���� �ֹ� ����: {pendingItem.itemName} x{pendingQuantity}");
        Debug.Log($"[PhoneOrderReceiver] ���õ� �ر� ���� resultId: {selectedRecipe.resultId}");
        Debug.Log($"[PhoneOrderReceiver] {incomingOrderDuration}�� �ȿ� ��ȭ�� �޾ƾ� �մϴ�.");
    }

    private void AcceptOrder()
    {
        if (OrderManager.Instance == null)
        {
            Debug.LogError("[PhoneOrderReceiver] OrderManager.Instance �� �����ϴ�.");
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

        Debug.Log($"[PhoneOrderReceiver] �ֹ� ���� �Ϸ�: {pendingItem.itemName} x{pendingQuantity}");

        ClearIncomingOrder();
    }

    private void ExpireIncomingOrder()
    {
        Debug.Log("[PhoneOrderReceiver] �ֹ� �ð��� ����Ǿ����ϴ�. �ֹ��� ��ҵ˴ϴ�.");

        if (gameStatusUI != null)
        {
            gameStatusUI.ReduceSatisfaction(missedOrderSatisfactionPenalty);
            Debug.Log($"[PhoneOrderReceiver] ��������� {missedOrderSatisfactionPenalty} ����");
        }
        else
        {
            Debug.LogWarning("[PhoneOrderReceiver] GameStatusUI�� ������� �ʾҽ��ϴ�.");
        }

        ClearIncomingOrder();
    }

    private void ClearIncomingOrder()
    {
        hasIncomingOrder = false;
        pendingItem = null;
        pendingQuantity = 0;
        remainingTime = 0f;

        pendingRewardGold = 0;
        pendingRewardExp = 0;
        pendingRewardInfluence = 0;
        pendingRewardSatisfaction = 0;
    }
}