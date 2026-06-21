using UnityEngine;
using System.Collections;

public class Salt : MonoBehaviour
{
    public SnailFollow snail;
    public float lifetime = 10f; // Increased default lifespan!

    void Start()
    {
        StartCoroutine(SaltLifeRoutine());
    }


    private bool isCollected = false;

    IEnumerator SaltLifeRoutine()
    {
        // Spawning cartoon animation
        transform.localScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;

        // Stretch up fast
        float elapsed = 0f;
        float duration = 0.2f;
        Vector3 stretchScale = new Vector3(0.5f, 1.5f, 1f);
        while (elapsed < duration)
        {
            if (isCollected) yield break;
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, stretchScale, elapsed / duration);
            yield return null;
        }

        // Squash down
        elapsed = 0f;
        duration = 0.15f;
        Vector3 squashScale = new Vector3(1.3f, 0.7f, 1f);
        while (elapsed < duration)
        {
            if (isCollected) yield break;
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(stretchScale, squashScale, elapsed / duration);
            yield return null;
        }

        // Boing back to normal
        elapsed = 0f;
        duration = 0.15f;
        while (elapsed < duration)
        {
            if (isCollected) yield break;
            elapsed += Time.deltaTime;
            transform.localScale = Vector3.Lerp(squashScale, targetScale, elapsed / duration);
            yield return null;
        }

        transform.localScale = targetScale;

        // Wait for lifetime
        elapsed = 0f;
        while (elapsed < lifetime)
        {
            if (isCollected) yield break;
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Despawn shrink
        while (transform.localScale.x > 0.05f)
        {
            if (isCollected) yield break;
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 8f);
            yield return null;
        }

        if (!isCollected) Destroy(gameObject);
    }

    public void Collect()
    {
        if (isCollected) return;
        isCollected = true;
        
        // Disable collider so it can't be clicked again during animation
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        StartCoroutine(CollectAnimationRoutine());
    }

    private IEnumerator CollectAnimationRoutine()
    {
        float elapsed = 0f;
        float duration = 0.2f;
        Vector3 startScale = transform.localScale;
        
        // Huge satisfying pop
        Vector3 popScale = new Vector3(1.5f, 1.5f, 1f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // Pop out fast, then shrink to 0
            if (elapsed < duration * 0.5f)
            {
                transform.localScale = Vector3.Lerp(startScale, popScale, (elapsed / (duration * 0.5f)));
            }
            else
            {
                transform.localScale = Vector3.Lerp(popScale, Vector3.zero, ((elapsed - (duration * 0.5f)) / (duration * 0.5f)));
            }
            yield return null;
        }

        Destroy(gameObject);
    }
}
