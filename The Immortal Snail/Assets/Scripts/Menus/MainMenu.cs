using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject Fadepanel;
    public Animator animator;

    void Start()
    {
        Fadepanel.SetActive(true);
        MenuMusic.instance?.FadeIn();
    }

    public void Play()
    {
        StartCoroutine(LoadLevel("MainLevel"));
    }

    public void HowToPlay()
    {
        StartCoroutine(LoadLevel("HowToPlay"));
    }

    public void Settings()
    {
        StartCoroutine(LoadLevel("Settings"));
    }

    public void Back()
    {
        StartCoroutine(LoadLevel("MainMenu"));
    }

    public void QuitGame()
    {
        StartCoroutine(LoadLevel("Quit"));
    }

    IEnumerator LoadLevel(string levelName)
    {
        animator.SetTrigger("Start");

        if (levelName == "MainLevel" && MenuMusic.instance != null)
        {
            MenuMusic.instance.FadeOutAndStop();
        }

        MenuMusic.instance.FadeOut();

        yield return new WaitForSeconds(2);

        if (levelName == "Quit")
        {
            Application.Quit();
        }
        else
        {
            SceneManager.LoadScene(levelName);
        }        
    }
}