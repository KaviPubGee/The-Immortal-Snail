using UnityEngine;
using System.Collections;

public class ThrowSalt : MonoBehaviour
{
    public float arcHeight = 2f;
    public float flightSpeed = 10f;

    [HideInInspector] public GameObject placedSaltPrefab;

    public void Toss(Vector2 startPos, Vector2 targetPos, GameObject prefabToSpawn)
    {
        placedSaltPrefab = prefabToSpawn;
        StartCoroutine(FlyInArc(startPos, targetPos));
    }

    IEnumerator FlyInArc(Vector2 startPos, Vector2 targetPos)
    {
        float distance = Vector2.Distance(startPos, targetPos);
        float duration = distance / flightSpeed;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float percentComplete = elapsed / duration;
            Vector2 currentFlatPosition = Vector2.Lerp(startPos, targetPos, percentComplete);

            // Sine wave gives a natural arc height
            float currentHeight = Mathf.Sin(percentComplete * Mathf.PI) * arcHeight;

            transform.position = new Vector3(currentFlatPosition.x, currentFlatPosition.y + currentHeight, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        if (placedSaltPrefab != null)
        {
            Instantiate(placedSaltPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
