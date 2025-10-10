using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSlotUI : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI countText;

    public Item item;   // 임시 담긴 아이템(1종류만)
    public int count;   // 담은 개수

    public void Set(Item it, int cnt = 1)
    {
        item = it; count = cnt;
        if (item != null)
        {
            icon.enabled = item.itemImage != null;
            icon.sprite = item.itemImage;
            countText.text = (count > 1) ? count.ToString() : "";
        }
        else { Clear(); }
    }

    public void Clear()
    {
        item = null; count = 0;
        if (icon) { icon.sprite = null; icon.enabled = false; }
        if (countText) countText.text = "";
    }

    public bool IsEmpty => item == null || count <= 0;
}
