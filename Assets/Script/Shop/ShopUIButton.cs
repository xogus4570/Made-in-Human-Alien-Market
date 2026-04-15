using UnityEngine;

public class ShopUIButton : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private Item item;

    public void Buy()
    {
        if (shop == null)
        {
            Debug.LogWarning("[ShopUIButton] Shop이 연결되지 않았습니다.");
            return;
        }

        if (item == null)
        {
            Debug.LogWarning("[ShopUIButton] Item이 연결되지 않았습니다.");
            return;
        }

        bool success = shop.BuyItem(item);

        if (success)
            Debug.Log($"[ShopUIButton] 구매 성공: {item.itemName}");
        else
            Debug.Log($"[ShopUIButton] 구매 실패: {item.itemName}");
    }
}