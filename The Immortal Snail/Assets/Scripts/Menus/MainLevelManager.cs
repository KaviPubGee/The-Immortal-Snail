using System.Collections;
using UnityEngine;

public class MainLevelManager : MonoBehaviour
{
    public GameObject Fadepanel;
    public Animator animator;
    public AudioSource gameMusic;
    public float fadeDuration = 1.5f;

    void Start()
    {
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.pixelDragThreshold = 20;
        }

        Fadepanel.SetActive(true);
        StartCoroutine(StartFadeNextFrame());

        if (gameMusic != null)
        {
            gameMusic.volume = 0f;
            if (!gameMusic.isPlaying) gameMusic.Play();
            StartCoroutine(FadeInMusic(0f, 1f, fadeDuration));
        }
    }

    IEnumerator FadeInMusic(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            gameMusic.volume = Mathf.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        gameMusic.volume = to;
    }

    IEnumerator StartFadeNextFrame()
    {
        // Wait for the laggy loading frame to completely finish
        yield return new WaitForEndOfFrame(); 
        
        // NOW start the animation
        animator.Play("FadeOut", -1, 0f);
    }
}
