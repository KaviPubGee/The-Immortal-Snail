using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("MainLevel");
    }

    public void HowToPlay()
    {
        SceneManager.LoadScene("HowToPlay");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
