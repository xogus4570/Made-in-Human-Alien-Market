using System;
using UnityEngine;

[Serializable]
public class ShopEntry
{
    public string entryId;
    public string displayName;
    public int buyPrice = 10;

    [Header("아이템 판매/해금용")]
    public string itemId;
    public int count = 1;

    [Header("강화용")]
    public string upgradeId;
}