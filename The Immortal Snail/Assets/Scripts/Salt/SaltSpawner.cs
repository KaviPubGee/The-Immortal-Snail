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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip spawnSound;

    [Header("Spawn Area")]
    public float minX = -7f;
    public float maxX = 7f;
    public float minY = -4f;
    public float maxY = 4f;

    [Header("Spawn Settings")]
    public float spawnDelay = 3.5f;
    public int maxSaltsOnScreen = 4; // Caps how many salts can exist at once!

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
            // Only spawn a new salt if the player hasn't let the map fill up too much
            int currentSaltCount = GameObject.FindGameObjectsWithTag("Salt").Length 
                                 + GameObject.FindGameObjectsWithTag("SnailSalt").Length;

            if (currentSaltCount < maxSaltsOnScreen)
            {
                SpawnSalt();

                if (!firstSaltSpawned)
                {
                    yield return new WaitForSeconds(0.5f);
                    firstSaltSpawned = true;
                }
            }
            
            // Wait the delay time, then check if we should spawn another!
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnSalt()
    {
        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }

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

        GameObject newSalt = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        Salt saltScript = newSalt.GetComponent<Salt>();

        if (saltScript != null)
        {
            saltScript.snail = snail;
        }
    }

    public void AddSaltCollected()
    {
        saltCollected ++;
    }
}