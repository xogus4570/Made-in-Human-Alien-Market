using System.Collections;
using UnityEngine;

public class InspectorController : MonoBehaviour
{
    [Header("추적 대상")]
    [SerializeField] private Transform player;

    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float stopDistance = 0.4f;

    [Header("방해 설정")]
    [SerializeField] private float hitDistance = 0.7f;
    [SerializeField] private float knockbackDistance = 1.2f;
    [SerializeField] private float waitAfterHit = 3f;

    private bool isWaiting;

    private void OnEnable()
    {
        isWaiting = false;
    }

    private void Update()
    {
        if (player == null) return;
        if (isWaiting) return;

        FollowPlayer();
        TryInterruptPlayer();
    }

    private void FollowPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= stopDistance)
            return;

        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    private void TryInterruptPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > hitDistance)
            return;

        PlayerInterruption interruption = player.GetComponent<PlayerInterruption>();

        if (interruption != null)
        {
            Vector2 knockbackDirection = (player.position - transform.position).normalized;
            interruption.Interrupt(knockbackDirection, knockbackDistance);
        }

        StartCoroutine(WaitAfterHitRoutine());
    }

    private IEnumerator WaitAfterHitRoutine()
    {
        isWaiting = true;

        yield return new WaitForSeconds(waitAfterHit);

        isWaiting = false;
    }

    public void SetPlayer(Transform target)
    {
        player = target;
    }
}