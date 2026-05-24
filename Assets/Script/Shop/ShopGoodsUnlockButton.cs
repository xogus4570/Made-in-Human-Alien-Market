using UnityEngine;

public class ShopGoodsUnlockButton : MonoBehaviour
{
    [SerializeField] private Shop shop;

    [Header("해금할 굿즈 ID 목록")]
    [SerializeField] private string[] goodsItemIds;

    [Header("가격")]
    [SerializeField] private int price = 500;

    public void BuyGoodsUnlock()
    {
        if (shop == null)
        {
            Debug.LogWarning("[ShopGoodsUnlockButton] Shop이 연결되지 않았습니다.");
            return;
        }

        if (goodsItemIds == null || goodsItemIds.Length == 0)
        {
            Debug.LogWarning("[ShopGoodsUnlockButton] 해금할 goodsItemIds가 없습니다.");
            return;
        }

        bool success = shop.BuyGoodsUnlock(goodsItemIds, price);

        if (success)
            Debug.Log("[ShopGoodsUnlockButton] 굿즈 묶음 해금 구매 성공");
        else
            Debug.Log("[ShopGoodsUnlockButton] 굿즈 묶음 해금 구매 실패");
    }
}