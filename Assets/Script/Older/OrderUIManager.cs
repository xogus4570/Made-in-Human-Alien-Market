using System.Collections.Generic;
using UnityEngine;

public class OrderUIManager : MonoBehaviour
{
    [SerializeField] private Transform orderListRoot;
    [SerializeField] private OrderCardUI orderCardPrefab;
    [SerializeField] private RecipeDB recipeDB;

    private readonly List<OrderCardUI> spawnedCards = new List<OrderCardUI>();

    private void Start()
    {
        if (OrderManager.Instance != null)
            OrderManager.Instance.OnOrderListChanged += RefreshUI;

        RefreshUI();
    }

    private void OnDestroy()
    {
        if (OrderManager.Instance != null)
            OrderManager.Instance.OnOrderListChanged -= RefreshUI;
    }

    private void RefreshUI()
    {
        ClearCards();

        if (OrderManager.Instance == null) return;

        IReadOnlyList<OrderData> orders = OrderManager.Instance.GetActiveOrders();

        for (int i = 0; i < orders.Count; i++)
        {
            OrderCardUI newCard = Instantiate(orderCardPrefab, orderListRoot);
            newCard.SetRecipeDB(recipeDB);
            newCard.SetOrder(orders[i]);
            spawnedCards.Add(newCard);
        }
    }

    private void ClearCards()
    {
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (spawnedCards[i] != null)
                Destroy(spawnedCards[i].gameObject);
        }

        spawnedCards.Clear();
    }
}
