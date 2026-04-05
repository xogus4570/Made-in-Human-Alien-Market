using System;
using UnityEngine;

[Serializable]
public class OrderData
{
    public string orderId;
    public Item orderedItem;
    public int quantity;
    public bool isCompleted;

    public int rewardGold;
    public int rewardExp;
    public int rewardInfluence;
    public int rewardSatisfaction;

    public OrderData(Item item, int qty, int gold, int exp, int influence, int satisfaction)
    {
        orderId = Guid.NewGuid().ToString();
        orderedItem = item;
        quantity = qty;
        isCompleted = false;

        rewardGold = gold;
        rewardExp = exp;
        rewardInfluence = influence;
        rewardSatisfaction = satisfaction;
    }
}
