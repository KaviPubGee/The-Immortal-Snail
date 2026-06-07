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


    IEnumerator SaltLifeRoutine()
    {
        // Start tiny
        transform.localScale = Vector3.zero;

        // Scale in
        while (transform.localScale.x < 0.95f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 8f);
            yield return null;
        }

        transform.localScale = Vector3.one;

        // Stay on screen for the lifetime duration
        yield return new WaitForSeconds(lifetime);

        // Scale out
        while (transform.localScale.x > 0.05f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 8f);
            yield return null;
        }

        Destroy(gameObject);
    }
}
