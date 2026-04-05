using UnityEngine;

public class DeliveryStorage : MonoBehaviour, IInteractable
{
    [SerializeField] private DeliveryInventoryUI deliveryInventoryUI;

    public string GetInteractionName()
    {
        return "창고 열기";
    }

    public void OnInteract(GameObject interactor)
    {
        if (deliveryInventoryUI == null)
        {
            Debug.LogWarning("[DeliveryStorage] deliveryInventoryUI가 연결되지 않았습니다.");
            return;
        }

        if (deliveryInventoryUI.IsOpen())
            deliveryInventoryUI.Close();
        else
            deliveryInventoryUI.Open();
    }
}