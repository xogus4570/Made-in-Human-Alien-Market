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

    // CheckerAnimationController가 읽을 검문관 이동 방향
    public Vector2 MoveDirection { get; private set; }

    private void OnEnable()
    {
        isWaiting = false;
        MoveDirection = Vector2.zero;
    }

    private void OnDisable()
    {
        MoveDirection = Vector2.zero;
    }

    private void Update()
    {
        if (GameFlowController.Instance == null)
        {
            MoveDirection = Vector2.zero;
            return;
        }

        if (GameFlowController.Instance.currentState != GameFlowState.Play)
        {
            MoveDirection = Vector2.zero;
            return;
        }

        if (player == null)
        {
            MoveDirection = Vector2.zero;
            return;
        }

        SaveInspectorPosition();

        if (isWaiting)
        {
            MoveDirection = Vector2.zero;
            return;
        }

        FollowPlayer();
        TryInterruptPlayer();

        SaveInspectorPosition();
    }

    private void FollowPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= stopDistance)
        {
            MoveDirection = Vector2.zero;
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;

        MoveDirection = direction;

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
        MoveDirection = Vector2.zero;

        yield return new WaitForSeconds(waitAfterHit);

        isWaiting = false;
    }

    public void SetPlayer(Transform target)
    {
        player = target;
    }

    private void SaveInspectorPosition()
    {
        if (GameDataManager.Instance == null)
            return;

        if (!gameObject.activeSelf)
            return;

        GameDataManager.Instance.inspectorPosition = transform.position;
        GameDataManager.Instance.hasInspectorPosition = true;
        GameDataManager.Instance.inspectorWasActive = true;
    }
}