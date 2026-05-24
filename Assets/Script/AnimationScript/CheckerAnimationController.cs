using UnityEngine;

public class CheckerAnimationController : MonoBehaviour
{
    private InspectorController inspectorController;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private string currentAnim = "";

    private void Awake()
    {
        inspectorController = GetComponent<InspectorController>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (inspectorController == null || animator == null || spriteRenderer == null)
            return;

        Vector2 move = inspectorController.MoveDirection;
        bool isMoving = move.sqrMagnitude > 0.01f;

        string nextAnim;

        if (!isMoving)
        {
            nextAnim = "Checker_Idle";
        }
        else
        {
            float absX = Mathf.Abs(move.x);
            float absY = Mathf.Abs(move.y);

            if (absY > absX)
            {
                nextAnim = move.y > 0 ? "Checker_Walk_Up" : "Checker_Walk_Down";
            }
            else
            {
                nextAnim = "Checker_Walk_Side";

                // 기존 Checker_Walk_Side 애니메이션을 좌우 반전해서 사용
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
        if (currentAnim == animName)
            return;

        currentAnim = animName;
        animator.Play(animName);
    }
}