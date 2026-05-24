using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public PlayerFollowMouse playerFollowMouse;
    public SaltSpawner saltSpawner;

    public TMP_Text dialogueText;
    public TypeWriterEffect typeWriter;

    public GameObject dialogueCanvas;

    public float typeSpeed = 0.04f;

    [TextArea]
    public string[] dialogueLines;

    private int currentLineIndex = 0;
    private bool playedSecondDialogue = false;

    void Start()
    {
        StartCoroutine(StartFirstDialogueAfterDelay());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (typeWriter.isTyping)
            {
                // If text is still typing, Enter skips it
                typeWriter.Skip();
            }
            else
            {
                // If text is done, Enter goes to next line
                NextLine();
            }
        }

        if (saltSpawner.firstSaltSpawned && !playedSecondDialogue)
        {
            StartCoroutine(StartSecondDialogue());
        }
    }

    IEnumerator StartFirstDialogueAfterDelay()
    {
        Time.timeScale = 0f;

        playerFollowMouse.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        dialogueCanvas.SetActive(false);

        yield return new WaitForSecondsRealtime(3f);

        dialogueCanvas.SetActive(true);

        ShowCurrentLine();
    }

    IEnumerator StartSecondDialogue()
    {
        Time.timeScale = 0f;

        playerFollowMouse.enabled = false;

        dialogueCanvas.SetActive(false);

        yield return new WaitForSecondsRealtime(1f);

        dialogueCanvas.SetActive(true);

        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        typeWriter.Run(dialogueLines[currentLineIndex], dialogueText, typeSpeed);
    }

    void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= dialogueLines.Length)
        {
            Debug.Log("Dialogue finished");
            dialogueCanvas.SetActive(false);
            Time.timeScale = 1f;
            playerFollowMouse.enabled = true;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            return;
        }

        ShowCurrentLine();
    }
}