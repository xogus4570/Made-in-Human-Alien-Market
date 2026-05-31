using UnityEngine;

public class PhoneOrderAutoSpawner : MonoBehaviour
{
    [Header("��ȭ�� (�ֹ� ���� ���)")]
    [SerializeField] private CustomerPhoneOrderReceiver receiver;

    [Header("�ֹ� ���� �ð� (��)")]
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

        // �̹� �ֹ� ������ ���� ����
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
        Debug.Log($"[PhoneOrderAutoSpawner] ���� �ֹ����� ��� �ð�: {timer:F1}��");
    }
}