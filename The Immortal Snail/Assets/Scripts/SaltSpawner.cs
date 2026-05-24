using UnityEngine;
using System.Collections;

public class SaltSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject saltPrefab;
    public GameObject snailSaltPrefab;
    public int saltCollected = 0;
    public bool snailSaltUnlocked = false;
    public SnailFollow snail;

    [Header("Spawn Area")]
    public float minX = -7f;
    public float maxX = 7f;
    public float minY = -4f;
    public float maxY = 4f;

    [Header("Spawn Settings")]
    public float spawnDelay = 2f;

    private GameObject currentSalt;

    public bool firstSaltSpawned = false;

    void Start()
    {
        StartCoroutine(SpawnSaltRoutine());
    }

    IEnumerator SpawnSaltRoutine()
    {
        yield return new WaitForSeconds(8f);

        while (true)
        {
            SpawnSalt();

            if (!firstSaltSpawned)
            {
                yield return new WaitForSeconds(0.5f);
                firstSaltSpawned = true;
            }

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
        GameObject prefabToSpawn;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        Vector2 spawnPosition = new Vector2(randomX, randomY);

        if(snailSaltUnlocked == false)
        {
            prefabToSpawn = saltPrefab;
        }
        else
        {
            float randomChance = Random.Range(0f, 1f);

            if (randomChance < 0.4f)
            {
                prefabToSpawn = snailSaltPrefab;
            }
            else
            {
                prefabToSpawn = saltPrefab;
            }
        }

        currentSalt = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        Salt saltScript = currentSalt.GetComponent<Salt>();

        if (saltScript != null)
        {
            saltScript.snail = snail;
        }
    }

    public void AddSaltCollected()
    {
        saltCollected ++;

        if (saltCollected >= 5 && snailSaltUnlocked == false)
        {
            snailSaltUnlocked = true;
            Debug.Log("The snail is evolving");
        }
    }
}