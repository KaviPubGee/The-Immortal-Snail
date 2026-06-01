using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.AI;


public class MainMenu : MonoBehaviour
{
    public float transitionSpeed = 2f;

    [Header("Transition")]
    public Transform snail;
    public MainMenuSnailFollow menuSnail;
    public RectTransform circleMask;

    private bool isTransitioning = false;

    public GameObject mouse;

    [Header("Bottom Panel Transition")]
    public RectTransform blackPanel;
    public Vector2 panelStartPosition;
    public Vector2 panelEndPosition;

    [Header("Fade Transition")]
    public Image fadePanel;
    [SerializeField] private float fadeDuration = 1.5f;


    void Start()
    {
        mouse.SetActive(true);
        
        Color c = fadePanel.color;
        c.a = 0f;
        fadePanel.color = c;
        fadePanel.gameObject.SetActive(false);
    }

    public void Play()
    {
        if (isTransitioning) return;

        StartCoroutine(PlayLoadGameTransition());
    }

    IEnumerator PlayLoadGameTransition()
    {
        isTransitioning = true;

        // Stop snail movement
        if (menuSnail != null)
        {
            menuSnail.enabled = false;
        }

        mouse.SetActive(false);

        // Save snail position
        Vector3 snailWorldPos = snail.position;

        snail.GetComponent<Animator>().enabled = false;

        // Move circle to snail position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(snailWorldPos);
        circleMask.position = screenPos;

        // Start big
        circleMask.localScale = Vector3.one * 10f;

        blackPanel.anchoredPosition = panelStartPosition;

        // Shrink circle
        while (circleMask.localScale.x > 0.01f)
        {
            circleMask.localScale = Vector3.Lerp(
                circleMask.localScale,
                Vector3.zero,
                Time.deltaTime * transitionSpeed
            );

            blackPanel.anchoredPosition = Vector2.Lerp(
                blackPanel.anchoredPosition,
                panelEndPosition,
                Time.deltaTime * transitionSpeed
            );

            yield return null;
        }

        DialogueManager.ResetDialogueFlags();
        SceneManager.LoadScene("MainLevel");
    }

    public void HowToPlay()
    {
        if (isTransitioning) return;
        StartCoroutine(FadeToScene("HowToPlay"));
    }

    public void Settings()
    {
        if (isTransitioning) return;
        StartCoroutine(FadeToScene("Settings"));
    }

    public void QuitGame()
    {
        StartCoroutine(FadeToScene("Quit"));
    }

    IEnumerator FadeToScene(string sceneName)
    {
        isTransitioning = true;

        mouse.SetActive(false);

        fadePanel.gameObject.SetActive(true);

        Color c = fadePanel.color;
        c.a = 0f;
        fadePanel.color = c;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float alpha = elapsedTime / fadeDuration;
            alpha = Mathf.Clamp01(alpha);

            c.a = alpha;
            fadePanel.color = c;

            yield return null;
        }

        c.a = 1f;
        fadePanel.color = c;

        if (sceneName == "Quit")
        {
            Application.Quit();
        }
        
        SceneManager.LoadScene(sceneName);
    }
}