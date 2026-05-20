using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float interactRadius = 1.0f;
    public LayerMask interactableMask;

    public Vector2 MoveInput { get; private set; }

    private Rigidbody2D rb;
    private Vector2 input;
    private PlayerInterruption playerInterruption;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInterruption = GetComponent<PlayerInterruption>();

        if (GameDataManager.Instance != null &&
            GameDataManager.Instance.hasPlayerPosition)
        {
            transform.position = GameDataManager.Instance.playerPosition;
            Debug.Log("[PlayerController] 저장된 플레이어 위치 복원");
        }
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        input = input.sqrMagnitude > 1f ? input.normalized : input;
        MoveInput = input;

        SavePlayerPosition();

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (playerInterruption != null && playerInterruption.IsInteractionBlocked)
            {
                Debug.Log("[PlayerController] 방해 상태라 상호작용할 수 없습니다.");
                return;
            }

            Collider2D target = Physics2D.OverlapCircle(transform.position, interactRadius, interactableMask);
            if (target != null)
                target.GetComponent<IInteractable>()?.OnInteract(gameObject);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * moveSpeed;
    }

    private void SavePlayerPosition()
    {
        if (GameDataManager.Instance == null)
            return;

        GameDataManager.Instance.playerPosition = transform.position;
        GameDataManager.Instance.hasPlayerPosition = true;
    }
}