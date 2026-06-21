using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public PlayerFollowMouse playerFollowMouse;

    [Header("Death Sequence")]
    public Animator panelAnimator; // The screen panel that fades to black
    public AudioSource levelMusic;
    public AudioSource deathMusic;
    public float fadeDuration = 1.5f;
    public GameObject snailObject;

    [HideInInspector] public bool hasDied = false;

    private CurseManager cachedCurseManager;

    void Start()
    {
        gameOverCanvas.SetActive(false);
        cachedCurseManager = FindFirstObjectByType<CurseManager>();
    }

    public void TriggerGameOver()
    {
        if (hasDied) return;
        hasDied = true;

        // Stop freezing the player mouse! We want the player to use their hand to click buttons!
        if (playerFollowMouse != null)
            playerFollowMouse.isFrozen = false; 

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        if (cachedCurseManager != null)
        {
            // We removed ForceClearUI() so that "INSTANT DEATH!" can remain on screen while fading to black
        }

        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        // 1. Freeze time
        Time.timeScale = 0f;

        // 2. Play fade to black visual (using your old trigger logic)
        if (panelAnimator != null)
            panelAnimator.SetTrigger("Start");

        // 3. Fade out level music over scaled time
        if (levelMusic != null)
            StartCoroutine(FadeMusicUnscaled(levelMusic, levelMusic.volume, 0f, fadeDuration));

        // 4. Wait for visual and audio fades to finish
        yield return new WaitForSecondsRealtime(fadeDuration);

        // --- WE ARE NOW IN PERFECT PITCH BLACK ---

        // 4.5 Hide the snail and wait a tiny bit in the dark
        if (snailObject != null)
            snailObject.SetActive(false);
        
        yield return new WaitForSecondsRealtime(1.0f); // Adjust this to wait longer or shorter in the void!

        // 5. Activate Game Over UI IN THE DARK
        gameOverCanvas.SetActive(true);

        // 5.5. PULL BACK THE CURTAIN! (Fade black screen back to transparent)
        if (panelAnimator != null)
            panelAnimator.Play("FadeOut", -1, 0f);

        // 6. Fade in Death music
        if (deathMusic != null)
        {
            deathMusic.volume = 0f;
            if(!deathMusic.isPlaying) deathMusic.Play();
            StartCoroutine(FadeMusicUnscaled(deathMusic, 0f, 1f, fadeDuration));
        }
    }

    IEnumerator FadeMusicUnscaled(AudioSource source, float startVol, float targetVol, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVol, targetVol, elapsed / duration);
            yield return null;
        }
        source.volume = targetVol;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}