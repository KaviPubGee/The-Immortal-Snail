using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealthBar : MonoBehaviour
{
    public Animator fadeAnimator;
    [HideInInspector]public bool isDying = false;

    public AudioSource gameMusic; // Drag your level music here
    public float fadeDuration = 5f;


    public Slider slider;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }


    public void SetHealth(int health)
    {
        slider.value = health;
    }

    void Update()
    {
        if (slider.value <= 0 && !isDying)
        {
            isDying = true;
            
            SnailEndingManager endingManager = FindFirstObjectByType<SnailEndingManager>();
            if (endingManager != null)
            {
                endingManager.TriggerEnding();
            }
            else
            {
                StartCoroutine(TransitionToThankYou()); // Fallback just in case
            }
        }
    }

    IEnumerator TransitionToThankYou()
    {
        // 1. Freeze the game
        Time.timeScale = 0f;

        // 2. Play the visual fade
        if (fadeAnimator != null)
        {
            fadeAnimator.Play("FadeOutSuperSlow");
        }

        // 3. Play the audio fade
        if (gameMusic != null) {
            StartCoroutine(FadeOutMusicUnscaled(gameMusic.volume, 0f, fadeDuration));
        }

        // 4. Wait for the duration
        yield return new WaitForSecondsRealtime(fadeDuration);

        // Reset the timeScale back to normal before loading a new scene!!!
        // Otherwise, the Thank You scene will load completely frozen!
        Time.timeScale = 1f;

        // 5. Load Thank You
        SceneManager.LoadScene("ThankYouScene"); 
    }

    IEnumerator FadeOutMusicUnscaled(float startVolume, float targetVolume, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // THIS is the secret sauce for fading audio while the game is paused!
            elapsed += Time.unscaledDeltaTime; 
            
            gameMusic.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
            yield return null;
        }
        gameMusic.volume = targetVolume;
    }

}
