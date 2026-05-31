using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuCanvas;

    public PlayerFollowMouse playerFollowMouse;
    public DialogueManager dialogueManager;
    public TypeWriterEffect typeWriter;

    public bool IsPaused { get; private set; } = false;

    void Start()
    {
        pauseMenuCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        IsPaused = true;

        pauseMenuCanvas.SetActive(true);

        if (dialogueManager != null && dialogueManager.IsDialogueActive)
        {
            dialogueManager.dialogueCanvas.SetActive(false);
        }

        if (typeWriter != null) typeWriter.Pause();

        Time.timeScale = 0f;

        if (playerFollowMouse != null)
        {
            playerFollowMouse.isFrozen = true;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        IsPaused = false;

        pauseMenuCanvas.SetActive(false);

        // IMPORTANT:
        // If dialogue is still active, stay frozen.
        if (dialogueManager != null && dialogueManager.IsDialogueActive)
        {
            Time.timeScale = 0f;

            if (playerFollowMouse != null)
            {
                playerFollowMouse.isFrozen = true;
            }

            dialogueManager.dialogueCanvas.SetActive(true);

            if (typeWriter != null) typeWriter.Resume();

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;

            return;
        }

        // Only resume normally if no dialogue is active.
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        if (playerFollowMouse != null)
        {
            playerFollowMouse.SyncWithRealMouse();
            playerFollowMouse.isFrozen = false;
        }
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        if (playerFollowMouse != null)
        {
            playerFollowMouse.isFrozen = false;
        }

        SceneManager.LoadScene("MainMenu");
    }
}