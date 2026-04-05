using UnityEngine;

public class PhoneOrderDebugTester : MonoBehaviour
{
    [SerializeField] private PhoneOrderReceiver phoneReceiver;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (phoneReceiver == null)
            {
                Debug.LogWarning("[PhoneOrderDebugTester] phoneReceiver가 연결되지 않았습니다.");
                return;
            }

            phoneReceiver.CreateTestIncomingOrder();
        }
    }
}
