using UnityEngine;

public class PhoneOrderAutoSpawner : MonoBehaviour
{
    [Header("전화기 (주문 생성 대상)")]
    [SerializeField] private PhoneOrderReceiver receiver;

    [Header("주문 생성 시간 (초)")]
    [SerializeField] private float minSpawnTime = 5f;
    [SerializeField] private float maxSpawnTime = 8f;

    private float timer;

    private void Start()
    {
        ResetSpawnTimer();
    }

    private void Update()
    {
        if (receiver == null) return;

        // 이미 주문 있으면 생성 안함
        if (receiver.HasIncomingOrder) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            receiver.CreateTestIncomingOrder();
            ResetSpawnTimer();
        }
    }

    private void ResetSpawnTimer()
    {
        timer = Random.Range(minSpawnTime, maxSpawnTime);
        Debug.Log($"[PhoneOrderAutoSpawner] 다음 주문까지 대기 시간: {timer:F1}초");
    }
}