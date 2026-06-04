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

        yield return new WaitForSeconds(2);

        if (levelName == "MainLevel")
        {
            if(MenuMusic.instance != null)
            {
                MenuMusic.instance.StopMusic();
            }
        }

        if (levelName == "Quit")
        {
            Application.Quit();
        }

        SceneManager.LoadScene(levelName);
    }
}