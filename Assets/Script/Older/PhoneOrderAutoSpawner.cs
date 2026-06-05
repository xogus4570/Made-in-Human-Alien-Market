using UnityEngine;

public class PhoneOrderAutoSpawner : MonoBehaviour
{
    [Header("Order Receiver")]
    [SerializeField] private CustomerPhoneOrderReceiver receiver;

    [Header("Spawn Time Seconds")]
    [SerializeField] private float minSpawnTime = 5f;
    [SerializeField] private float maxSpawnTime = 8f;

    private float timer;

    private void Start()
    {
        ResetSpawnTimer();
    }

    private void Update()
    {
        if (receiver == null)
        {
            Debug.LogWarning("[PhoneOrderAutoSpawner] Receiver is not null.");
            return;
        }

        if (receiver.HasIncomingOrder)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            Debug.Log("[PhoneOrderAutoSpawner] 주문 생성 시도");
            receiver.CreateTestIncomingOrder();
            ResetSpawnTimer();
        }
    }

    private void ResetSpawnTimer()
    {
        timer = Random.Range(minSpawnTime, maxSpawnTime);

        Debug.Log($"[PhoneOrderAutoSpawner] 다음 주문 생성까지: {timer:F1}초");

        Debug.Log($"[PhoneOrderAutoSpawner] Next order in {timer:F1} seconds.");

    }
}