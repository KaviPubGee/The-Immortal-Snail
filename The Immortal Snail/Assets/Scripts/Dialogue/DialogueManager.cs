using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public AudioSource dialougeSound;

    public PlayerFollowMouse playerFollowMouse;
    public SaltSpawner saltSpawner;
    public PlayerCollision playerCollision;
    public PauseMenu pauseMenu;

    public TMP_Text dialogueText;
    public TypeWriterEffect typeWriter;

    public GameObject dialogueCanvas;

    [Header("Snail Pictures")]
    public RawImage snailPicture;
    public Texture2D snailScared;
    public Texture2D snailNormal;
    public Texture2D snailMad;
    public Texture2D snailConfused;
    public Texture2D snailDemonic;
    public Texture2D snailLaugh;
    public Texture2D snailCough;
    public Texture2D snailPain;


    [Header("")]
    public float typeSpeed = 0.04f;

    public float waitTime = 5f;

    [Header("Dialogues")]
    [TextArea] public string[] introDialogue;
    [TextArea] public string[] firstSaltDialogue;
    [TextArea] public string[] firstSaltDialogueAfterCollecting;
    [TextArea] public string[] firstDialogueAfterCollectingFive;
    [TextArea] public string[] firstDialogueAfterCursedSalt;

    private string[] currentDialogue;
    private int currentLineIndex = 0;

    private bool dialogueActive = false;
    public bool IsDialogueActive
    {
        get{return dialogueActive;}
    }

    private static bool playedFirstSaltDialogue = false;
    private static bool playedFirstSaltDialogueAfterCollecting = false;
    private static bool playedFirstDialogueAfterCollectingFive = false;
    private static bool playedFirstDialogueAfterCursedSalt = false;
    private static bool playedIntroDialogue = false;

    void Start()
    {
        dialogueCanvas.SetActive(false);
        StartCoroutine(StartIntroAfterDelay());
    }

    void Update()
    {
        if (pauseMenu != null && pauseMenu.IsPaused)
        {
            // Stop audio while paused
            if (dialougeSound.isPlaying)
                dialougeSound.Stop();
            return;
        }

        // Resume audio if typewriter is still typing after unpause
        if (dialogueActive && typeWriter.isTyping && !dialougeSound.isPlaying)
            dialougeSound.Play();

        if (saltSpawner.firstSaltSpawned && !playedFirstSaltDialogue && !dialogueActive)
        {
            playedFirstSaltDialogue = true;
            snailPicture.texture = snailScared;
            StartDialogue(firstSaltDialogue);
        }

        if (playerCollision.snailHitsWithSalt >= 1 && !playedFirstSaltDialogueAfterCollecting && !dialogueActive)
        {
            playedFirstSaltDialogueAfterCollecting = true;
            snailPicture.texture = snailScared;
            StartDialogue(firstSaltDialogueAfterCollecting);
        }

        if (playerCollision.snailHitsWithSalt >= 5 && !playedFirstDialogueAfterCollectingFive && !dialogueActive)
        {
            playedFirstDialogueAfterCollectingFive = true;
            snailPicture.texture = snailMad;
            StartDialogue(firstDialogueAfterCollectingFive);
        }

        if (playerCollision.pickedUpCursedSaltFirstTime && !playedFirstDialogueAfterCursedSalt && !dialogueActive)
        {
            playedFirstDialogueAfterCursedSalt = true;
            snailPicture.texture = snailLaugh;
            StartDialogue(firstDialogueAfterCursedSalt);
        }

        if (!dialogueActive) return;

        if (!typeWriter.isTyping && dialougeSound.isPlaying)
        {
            dialougeSound.Stop();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (typeWriter.isTyping)
            {
                typeWriter.Skip();
            }
            else
            {
                NextLine();
            }
        }
    }

    public void LowerVolume()
    {
        
    }

    public static void ResetDialogueFlags()
    {
        playedIntroDialogue = false;
        playedFirstSaltDialogue = false;
        playedFirstSaltDialogueAfterCollecting = false;
        playedFirstDialogueAfterCollectingFive = false;
        playedFirstDialogueAfterCursedSalt = false;
    }

    IEnumerator StartIntroAfterDelay()
    {
        if (playedIntroDialogue)
            yield break;

        playedIntroDialogue = true;

        Time.timeScale = 0f;
        playerFollowMouse.enabled = false;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        yield return new WaitForSecondsRealtime(waitTime);

        while (pauseMenu != null && pauseMenu.IsPaused)
        {
            yield return null;
        }

        StartDialogue(introDialogue);
    }

    public void StartDialogue(string[] dialogueToPlay)
    {
        currentDialogue = dialogueToPlay;
        currentLineIndex = 0;
        dialogueActive = true;

        Time.timeScale = 0f;
        playerFollowMouse.enabled = false;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        dialogueCanvas.SetActive(true);

        ShowCurrentLine();
    }

    void ShowCurrentLine()
    {
        if (currentDialogue == firstSaltDialogueAfterCollecting)
        {
            switch (currentLineIndex)
            {
                case 0:
                    snailPicture.texture = snailPain;
                    break;
                
                case 1:
                    snailPicture.texture = snailNormal;
                    break;
            }
        }

        if (currentDialogue == firstDialogueAfterCollectingFive)
        {
            switch (currentLineIndex)
            {
                case 0:
                    snailPicture.texture = snailMad;
                    break;

                case 1:
                    snailPicture.texture = snailDemonic;
                    dialogueText.color = Color.red;
                    break;

                case 2:
                    snailPicture.texture = snailDemonic;
                    dialogueText.color = Color.white;
                    break;

                case 3:
                    snailPicture.texture = snailDemonic;
                    break;

                case 4:
                    snailPicture.texture = snailDemonic;
                    break;

                case 5:
                    snailPicture.texture = snailConfused;
                    break;

                case 6:
                    snailPicture.texture = snailConfused;
                    break;

                case 7:
                    snailPicture.texture = snailNormal;
                    break;
            }
        }

        if (currentDialogue == firstDialogueAfterCursedSalt)
        {
            switch (currentLineIndex)
            {
                case 0:
                    snailPicture.texture = snailNormal;
                    break;

                case 1:
                    snailPicture.texture = snailLaugh;
                    break;

                case 2:
                    snailPicture.texture = snailLaugh;
                    break;

                case 3:
                    snailPicture.texture = snailLaugh;
                    break;

                case 4:
                    snailPicture.texture = snailCough;
                    break;

                case 5:
                    snailPicture.texture = snailCough;
                    break;
            }
        }

        dialougeSound.Play();
        typeWriter.Run(currentDialogue[currentLineIndex], dialogueText, typeSpeed);
    }

    void NextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= currentDialogue.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    void EndDialogue()
    {
        dialougeSound.Stop();

        Debug.Log("Dialogue finished");

        dialogueActive = false;

        dialogueCanvas.SetActive(false);

        Time.timeScale = 1f;
        playerFollowMouse.enabled = true;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }
}