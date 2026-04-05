using UnityEngine;

public class PhoneOrderReceiver : MonoBehaviour, IInteractable
{
    [Header("랜덤 주문용 레시피 DB")]
    [SerializeField] private RecipeDB recipeDB;

    [Header("주문 수량 범위")]
    [SerializeField] private int minOrderQuantity = 1;
    [SerializeField] private int maxOrderQuantity = 1;

    [Header("주문 제한시간")]
    [SerializeField] private float incomingOrderDuration = 10f;

    [Header("실패 패널티")]
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

        if (recipeDB.recipes == null || recipeDB.recipes.Count == 0)
        {
            Debug.LogError("[PhoneOrderReceiver] RecipeDB에 등록된 레시피가 없습니다.");
            return;
        }

        // 유효한 resultId를 가진 레시피를 찾기 위해 몇 번 시도
        Recipe selectedRecipe = null;
        const int maxTry = 20;

        for (int i = 0; i < maxTry; i++)
        {
            int randomIndex = Random.Range(0, recipeDB.recipes.Count);
            Recipe candidate = recipeDB.recipes[randomIndex];

            if (candidate == null) continue;
            if (string.IsNullOrEmpty(candidate.resultId)) continue;

            Item resultItem = ItemDataBase.instance.GetById(candidate.resultId);
            if (resultItem == null) continue;

            selectedRecipe = candidate;
            break;
        }

        if (selectedRecipe == null)
        {
            Debug.LogError("[PhoneOrderReceiver] 유효한 resultId를 가진 레시피를 찾지 못했습니다.");
            return;
        }

        Item item = ItemDataBase.instance.GetById(selectedRecipe.resultId);
        if (item == null)
        {
            Debug.LogError($"[PhoneOrderReceiver] resultId '{selectedRecipe.resultId}' 아이템을 찾을 수 없습니다.");
            return;
        }

        hasIncomingOrder = true;
        pendingItem = item;
        pendingQuantity = Random.Range(
            Mathf.Max(1, minOrderQuantity),
            Mathf.Max(1, maxOrderQuantity) + 1
        );
        remainingTime = incomingOrderDuration;

        // [추가] 레시피 보상값 임시 저장
        pendingRewardGold = selectedRecipe.rewardGold;
        pendingRewardExp = selectedRecipe.rewardExp;
        pendingRewardInfluence = selectedRecipe.rewardInfluence;
        pendingRewardSatisfaction = selectedRecipe.rewardSatisfaction;

        // [추가] 보상값 미설정 경고 로그
        if (selectedRecipe.rewardGold == 0 &&
            selectedRecipe.rewardExp == 0 &&
            selectedRecipe.rewardInfluence == 0 &&
            selectedRecipe.rewardSatisfaction == 0)
        {
            Debug.LogWarning($"[Recipe] 보상 설정 안됨: {selectedRecipe.resultId}");
        }

        Debug.Log($"[PhoneOrderReceiver] 랜덤 주문 도착: {pendingItem.itemName} x{pendingQuantity}");
        Debug.Log($"[PhoneOrderReceiver] 선택된 레시피 resultId: {selectedRecipe.resultId}");
        Debug.Log($"[PhoneOrderReceiver] {incomingOrderDuration}초 안에 전화를 받아야 합니다.");
    }

    private void AcceptOrder()
    {
        if (OrderManager.Instance == null)
        {
            Debug.LogError("[PhoneOrderReceiver] OrderManager.Instance 가 없습니다.");
            return;
        }

        OrderManager.Instance.AddOrder(pendingItem, pendingQuantity, pendingRewardGold, pendingRewardExp, pendingRewardInfluence, pendingRewardSatisfaction);

        Debug.Log($"[PhoneOrderReceiver] 주문 수락 완료: {pendingItem.itemName} x{pendingQuantity}");

        ClearIncomingOrder();
    }

    private void ExpireIncomingOrder()
    {
        Debug.Log("[PhoneOrderReceiver] 주문 시간이 만료되었습니다. 주문이 취소됩니다.");

        if (gameStatusUI != null)
        {
            gameStatusUI.ReduceSatisfaction(missedOrderSatisfactionPenalty);
            Debug.Log($"[PhoneOrderReceiver] 고객만족도 {missedOrderSatisfactionPenalty} 감소");
        }
        else
        {
            Debug.LogWarning("[PhoneOrderReceiver] GameStatusUI가 연결되지 않았습니다.");
        }

        ClearIncomingOrder();
    }

    private void ClearIncomingOrder()
    {
        hasIncomingOrder = false;
        pendingItem = null;
        pendingQuantity = 0;
        remainingTime = 0f;
    }
}