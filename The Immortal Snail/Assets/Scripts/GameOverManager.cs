using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverCanvas;
    public PlayerFollowMouse playerFollowMouse;

    void Start()
    {
        gameOverCanvas.SetActive(false);
    }

    public void TriggerGameOver()
    {
        gameOverCanvas.SetActive(true);

        Time.timeScale = 0f;

        if (playerFollowMouse != null)
            playerFollowMouse.isFrozen = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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