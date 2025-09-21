using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;
    public float interactRadius = 1.0f; //탐지 범위
    public LayerMask interactableMask; // 레이어 체크

    private Rigidbody2D rb;
    private Vector2 input;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input = input.sqrMagnitude > 1f ? input.normalized : input;// 대각선 이동시 속도 보정 input.sqrMagnitude<- 벡터 길이 제곱


        if (Input.GetKeyDown(KeyCode.F))
        {
            Collider2D target = Physics2D.OverlapCircle(transform.position, interactRadius, interactableMask);
            if (target != null)
            {
                target.GetComponent<IInteractable>()?.OnInteract(this.gameObject); 
            }

        }
       
    }

    void FixedUpdate()
    {
        rb.velocity = input * moveSpeed;
    }
}
