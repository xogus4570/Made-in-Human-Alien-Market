using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float interactRadius = 1.0f;
    public LayerMask interactableMask;

    public Vector2 MoveInput { get; private set; }

    private Rigidbody2D rb;
    private Vector2 input;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        input = input.sqrMagnitude > 1f ? input.normalized : input;
        MoveInput = input;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Collider2D target = Physics2D.OverlapCircle(transform.position, interactRadius, interactableMask);
            if (target != null)
                target.GetComponent<IInteractable>()?.OnInteract(gameObject);
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input * moveSpeed;
    }
}