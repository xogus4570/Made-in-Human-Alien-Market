using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ItemType
{
    Goods,
    Material,
    Gem
}

[System.Serializable]
public class Item
{
    [Tooltip("프로젝트 전역에서 유일한 ID (예: potion_small, iron_ore)")]
    public string id;

    public ItemType itemType;
    public string itemName;
    public Sprite itemImage;

    public bool Use() => false;
}
