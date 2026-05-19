using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private PlayerController playerController;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isMasked = false;
    private string currentAnim = "";

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Vector2 move = playerController.MoveInput;
        bool isMoving = move.sqrMagnitude > 0.01f;

        string prefix = isMasked ? "Masked" : "Human";
        string nextAnim;

        if (!isMoving)
        {
            nextAnim = $"{prefix}_Idle";
        }
        else
        {
            float absX = Mathf.Abs(move.x);
            float absY = Mathf.Abs(move.y);

            if (absY > absX)
            {
                nextAnim = move.y > 0 ? $"{prefix}_Walk_Up" : $"{prefix}_Walk_Down";
            }
            else
            {
                nextAnim = $"{prefix}_Walk_Side";

                if (move.x > 0)
                    spriteRenderer.flipX = true;
                else if (move.x < 0)
                    spriteRenderer.flipX = false;
            }
        }

        PlayAnimation(nextAnim);
    }

    private void PlayAnimation(string animName)
    {
        if (currentAnim == animName) return;

        currentAnim = animName;
        animator.Play(animName);
    }

    public void SetMasked(bool value)
    {
        isMasked = value;
        currentAnim = "";
    }
}
