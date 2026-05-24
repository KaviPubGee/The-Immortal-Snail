using UnityEngine;
using System.Collections;

public class Salt : MonoBehaviour
{
    public SnailFollow snail;

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

        // Stay for 5 seconds
        yield return new WaitForSeconds(1f);

        // Scale out
        while (transform.localScale.x > 0.05f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 8f);
            yield return null;
        }

        Destroy(gameObject);
    }
}
