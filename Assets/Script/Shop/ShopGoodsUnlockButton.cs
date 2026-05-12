using UnityEngine;

public class ShopGoodsUnlockButton : MonoBehaviour
{
    [SerializeField] private Shop shop;
    [SerializeField] private string goodsItemId;
    [SerializeField] private int price = 500;

    public void BuyGoodsUnlock()
    {
        if (shop == null)
        {
            Debug.LogWarning("[ShopGoodsUnlockButton] Shop이 연결되지 않았습니다.");
            return;
        }

        if (string.IsNullOrEmpty(goodsItemId))
        {
            Debug.LogWarning("[ShopGoodsUnlockButton] goodsItemId가 비어 있습니다.");
            return;
        }

        bool success = shop.BuyGoodsUnlock(goodsItemId, price);

        if (success)
            Debug.Log($"[ShopGoodsUnlockButton] 굿즈 해금 구매 성공: {goodsItemId}");
        else
            Debug.Log($"[ShopGoodsUnlockButton] 굿즈 해금 구매 실패: {goodsItemId}");
    }
}