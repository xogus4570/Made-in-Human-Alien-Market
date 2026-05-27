using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomerOrderProfile
{
    [Header("손님 정보")]
    public string customerName;

    [TextArea]
    public string dialogMessage;

    public Sprite customerSprite;

    [Header("이 손님이 주문 가능한 결과물 ID")]
    public List<string> orderableResultIds =
        new List<string>();
}