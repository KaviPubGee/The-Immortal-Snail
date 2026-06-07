using UnityEngine;
using System.Collections;

public class PlacedSalt : MonoBehaviour
{
    public float lifetime = 4f;       // How long it stays fully visible on the ground
    public float fadeDuration = 2f;   // How long it takes to fade into nothingness

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip landSound;

    void Start()
    {
        if (audioSource != null && landSound != null)
            audioSource.PlayOneShot(landSound);

        StartCoroutine(FadeAndDie());
    }

    IEnumerator FadeAndDie()
    {
        // Wait on the ground as a trap
        yield return new WaitForSeconds(lifetime);

        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        if (spr != null)
        {
            Color startColor = spr.color;
            float elapsed = 0f;
            
            // Slowly fade the alpha to 0
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float newAlpha = Mathf.Lerp(startColor.a, 0f, elapsed / fadeDuration);
                spr.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
                yield return null;
            }
        }
        
        // Remove it from the game once invisible
        Destroy(gameObject);
    }
}
