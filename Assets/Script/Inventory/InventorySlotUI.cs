using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;   // 수량 표시용 

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI")]
    public Image icon;       // 아이템 아이콘
    public TextMeshProUGUI countText; // 수량 표시용 텍스트

    [Header("Data")]
    public Item item;        // 슬롯에 들어간 아이템
    public int count;        // 아이템 개수

    /// <summary>
    /// 슬롯에 아이템 세팅
    /// </summary>
    public void SetItem(Item newItem, int newCount = 1)
    {
        item = newItem; 
        count = newCount;

        if (item != null)
        {
            if (icon != null) 
            { 
                icon.sprite = item.itemImage; 
                icon.enabled = (item.itemImage != null); 
            }
            if (countText != null)
                countText.text = count.ToString();
        }
        else 
        { 
            Clear(); 
        }
        Debug.Log($"[SetItem] id={item?.id}, sprite={(item?.itemImage != null ? "OK" : "NULL")}");
    }

    /// <summary>
    /// 슬롯 비우기
    /// </summary>
    public void Clear()
    {
        item = null;
        count = 0;
        if (icon) { icon.sprite = null; icon.enabled = false; }
        if (countText) countText.text = "";
    }

    /// <summary>
    /// 슬롯 갱신 (개수만 변할 때)
    /// </summary>
    public void Refresh()
    {
        if (item != null)
        {
            countText.text = (count > 1) ? count.ToString() : "";
        }
        else
        {
            countText.text = "";
        }
    }
}
