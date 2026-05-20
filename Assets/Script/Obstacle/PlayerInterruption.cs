using System.Collections;
using UnityEngine;

public class PlayerInterruption : MonoBehaviour
{
    [Header("방해 상태")]
    [SerializeField] private float interactionBlockTime = 2f;
    [SerializeField] private float knockbackDuration = 0.15f;

    public bool IsInteractionBlocked { get; private set; }

    private Coroutine blockCoroutine;
    private Coroutine knockbackCoroutine;

    public void Interrupt(Vector2 knockbackDirection, float knockbackDistance)
    {
        if (knockbackDirection.sqrMagnitude <= 0f)
            knockbackDirection = Vector2.down;

        knockbackDirection.Normalize();

        if (knockbackCoroutine != null)
            StopCoroutine(knockbackCoroutine);

        knockbackCoroutine = StartCoroutine(KnockbackRoutine(knockbackDirection, knockbackDistance));

        if (blockCoroutine != null)
            StopCoroutine(blockCoroutine);

        blockCoroutine = StartCoroutine(BlockInteractionRoutine());
    }

    private IEnumerator KnockbackRoutine(Vector2 direction, float distance)
    {
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + (Vector3)(direction * distance);

        float elapsed = 0f;

        while (elapsed < knockbackDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / knockbackDuration;

            transform.position = Vector3.Lerp(startPos, targetPos, t);

            yield return null;
        }

        transform.position = targetPos;
        knockbackCoroutine = null;
    }

    private IEnumerator BlockInteractionRoutine()
    {
        IsInteractionBlocked = true;

        yield return new WaitForSeconds(interactionBlockTime);

        IsInteractionBlocked = false;
        blockCoroutine = null;
    }
}