using UnityEngine;
using System.Collections;

public class SaltSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject saltPrefab;
    public SnailFollow snail;

    [Header("Spawn Area")]
    public float minX = -7f;
    public float maxX = 7f;
    public float minY = -4f;
    public float maxY = 4f;

    [Header("Spawn Settings")]
    public float spawnDelay = 2f;

    private GameObject currentSalt;

    void Start()
    {
        StartCoroutine(SpawnSaltRoutine());
    }

    IEnumerator SpawnSaltRoutine()
    {
        while (true)
        {
            SpawnSalt();

            //Wait while salt exists
            while (currentSalt != null)
            {
                yield return null;
            }
            
            //Salt is gone, now wait before the next spawn
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnSalt()
    {
        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        Vector2 spawnPosition = new Vector2(randomX, randomY);

        currentSalt = Instantiate(saltPrefab, spawnPosition, Quaternion.identity);

        Salt saltScript = currentSalt.GetComponent<Salt>();

        if (saltScript != null)
        {
            saltScript.snail = snail;
        }
    }
}