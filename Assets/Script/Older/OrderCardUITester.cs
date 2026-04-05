using UnityEngine;

public class OrderCardUITester : MonoBehaviour
{
    [SerializeField] private OrderCardUI orderCardUI;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (orderCardUI == null)
            {
                Debug.LogWarning("[OrderCardUITester] orderCardUI가 연결되지 않았습니다.");
                return;
            }

            if (OrderManager.Instance == null)
            {
                Debug.LogWarning("[OrderCardUITester] OrderManager.Instance가 없습니다.");
                return;
            }

            var orders = OrderManager.Instance.GetActiveOrders();
            if (orders.Count > 0)
            {
                orderCardUI.SetOrder(orders[0]);
                Debug.Log("[OrderCardUITester] 첫 번째 주문을 카드에 표시했습니다.");
            }
            else
            {
                Debug.Log("[OrderCardUITester] 현재 활성 주문이 없습니다.");
            }
        }
    }
}
