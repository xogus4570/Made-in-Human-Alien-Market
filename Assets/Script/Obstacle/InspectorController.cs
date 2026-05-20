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

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private string currentAnim;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        isWaiting = false;

        if (GameDataManager.Instance != null &&
            GameDataManager.Instance.hasInspectorPosition)
        {
            transform.position = GameDataManager.Instance.inspectorPosition;
            Debug.Log("[InspectorController] 저장된 검문관 위치 복원");
        }

        SaveInspectorPosition();
        UpdateAnimation(Vector2.zero);
    }

    private void Update()
    {
        if (GameFlowController.Instance == null)
        {
            UpdateAnimation(Vector2.zero);
            return;
        }

        if (GameFlowController.Instance.currentState != GameFlowState.Play)
        {
            UpdateAnimation(Vector2.zero);
            return;
        }

        if (player == null)
        {
            UpdateAnimation(Vector2.zero);
            return;
        }

        SaveInspectorPosition();

        if (isWaiting)
        {
            UpdateAnimation(Vector2.zero);
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
            UpdateAnimation(Vector2.zero);
            return;
        }

        Vector2 direction = (player.position - transform.position).normalized;

        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

        UpdateAnimation(direction);
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
        UpdateAnimation(Vector2.zero);

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

        GameDataManager.Instance.inspectorPosition = transform.position;
        GameDataManager.Instance.hasInspectorPosition = true;
        GameDataManager.Instance.inspectorWasActive = gameObject.activeSelf;
    }

    private void UpdateAnimation(Vector2 dir)
    {
        if (animator == null)
            return;

        if (dir.sqrMagnitude < 0.01f)
        {
            PlayAnimation("Checker_Idle");
            return;
        }

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            PlayAnimation("Checker_Walk_Side");

            if (spriteRenderer != null)
            {
                // 원본 Side 스프라이트가 왼쪽을 보고 있어서 오른쪽 이동 시 뒤집음
                spriteRenderer.flipX = dir.x > 0;
            }
        }
        else
        {
            if (dir.y > 0)
            {
                PlayAnimation("Checker_Walk_Up");
            }
            else
            {
                PlayAnimation("Checker_Walk_Down");
            }
        }
    }

    private void PlayAnimation(string animName)
    {
        if (currentAnim == animName)
            return;

        currentAnim = animName;
        animator.Play(animName);
    }
}