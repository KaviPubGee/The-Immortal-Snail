using System.Collections;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public static MenuMusic instance;

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;

    private AudioSource audioSource;
    private Coroutine currentFade;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0f;

        if (!audioSource.isPlaying)
            audioSource.Play();

        FadeIn(); // fade in on first load
    }

    // Call this from each menu scene's Start() to re-fade in
    // Fades from current volume so it doesn't snap to 0 if already playing
    public void FadeIn()
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeVolume(audioSource.volume, 1f, fadeDuration));
    }

    public void FadeOut()
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeVolume(audioSource.volume, 0f, fadeDuration));
    }


    // Call this before loading MainLevel
    public void FadeOutAndStop()
    {
        if (currentFade != null) StopCoroutine(currentFade);
        currentFade = StartCoroutine(FadeOutCoroutine());
    }

    IEnumerator FadeOutCoroutine()
    {
        yield return FadeVolume(audioSource.volume, 0f, fadeDuration);
        Destroy(gameObject);
    }

    IEnumerator FadeVolume(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            audioSource.volume = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = to;
    }

    public void StopMusic()
    {
        Destroy(gameObject);
    }
}
