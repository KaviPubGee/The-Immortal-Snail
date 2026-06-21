using UnityEngine;
using System.Collections;

public class SnailEndingManager : MonoBehaviour
{
    [Header("QTE Settings")]
    [HideInInspector] public KeyCode[] qteSequence; // Now generated dynamically
    public int sequenceLength = 10;
    public float timeLimit = 8f;
    public GameObject bucketPrefab;
    
    [Header("QTE UI Setup")]
    public GameObject qteUIPanel;
    public UnityEngine.UI.Image[] qteButtonImages; // Drag 10 UI Images here!
    
    [Header("Arrow Sprites")]
    public Sprite upArrowSprite;
    public Sprite downArrowSprite;
    public Sprite leftArrowSprite;
    public Sprite rightArrowSprite;
    
    [Header("References")]
    public DialogueManager dialogueManager;
    public HealthBar snailHealthBar;
    public Transform snailTransform;
    public KeyCode debugTriggerKey = KeyCode.F5;

    [Header("Dialogues")]
    [TextArea] public string[] preQteDialogue = new string[] { "Urgh... I dont feel so good" };
    [TextArea] public string[] failQteDialogue = new string[] { "Phew. I almost got knocked out back there." };
    
    [Header("Phase 3 & Escape")]
    public Texture2D bucketDialoguePicture;
    [TextArea] public string[] trappedDialogue = new string[] { 
        "I... I'm not done with you as soon as i figure out how to get out of here.", 
        "And maybe after a quick nap..." 
    };
    public GameObject brokenWallOverlay;
    [TextArea] public string[] brokenWallDialogue = new string[] {
        "WHAT NO!!! DONT GO THERE DONT YOU LEAVE ME HERE!"
    };

    private int currentQteIndex = 0;
    private bool qteActive = false;
    private float qteTimer = 0f;
    private static bool playedDialoguesThisSession = false;
    private bool endingTriggered = false;
    private bool isResetting = false;
    private bool escapePhaseActive = false;
    private PauseMenu pauseMenu;

    void Start()
    {
        pauseMenu = FindFirstObjectByType<PauseMenu>();
    }

    void Update()
    {
        // Debug Key to trigger ending sequence
        if (Input.GetKeyDown(debugTriggerKey))
        {
            if (snailHealthBar != null && !endingTriggered) 
            {
                snailHealthBar.slider.value = 0; // Triggers the HealthBar Update() logic natively
            }
        }

        // Possessed Escape Logic (Taking control from the player completely)
        if (escapePhaseActive)
        {
            PlayerFollowMouse player = FindFirstObjectByType<PlayerFollowMouse>();
            if (player != null)
            {
                player.enabled = false; // Ensure they can't use the mouse!
                
                // The hand gets possessed and slowly drags to the RIGHT side off the screen!
                player.transform.position += Vector3.right * 3f * Time.deltaTime;
                
                // Slowly center the hand on the Y axis as it's being dragged
                player.transform.position = new Vector3(player.transform.position.x, Mathf.Lerp(player.transform.position.y, 0f, 2f * Time.deltaTime), player.transform.position.z);
            }
        }

        // QTE Logic
        if (qteActive && !isResetting)
        {
            // Pause the QTE completely if the game is paused via PauseMenu
            if (pauseMenu != null && pauseMenu.IsPaused) return;

            qteTimer -= Time.unscaledDeltaTime;
            
            // Fail if time runs out
            if (qteTimer <= 0)
            {
                FailQTE();
                return;
            }

            if (Input.anyKeyDown)
            {
                // Ignore mouse inputs for the QTE
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) return;

                // Safety check
                if (qteSequence == null || qteSequence.Length == 0) return;

                if (Input.GetKeyDown(qteSequence[currentQteIndex]))
                {
                    // Animate the pressed button image instead of hiding it
                    if (qteButtonImages != null && currentQteIndex < qteButtonImages.Length && qteButtonImages[currentQteIndex] != null)
                    {
                        StartCoroutine(AnimateButtonPress(qteButtonImages[currentQteIndex]));
                    }

                    currentQteIndex++;
                    
                    if (currentQteIndex >= qteSequence.Length)
                    {
                        SucceedQTE();
                    }
                }
                else
                {
                    // WRONG KEY! Reset progress to the start.
                    ResetQTEProgress();
                }
            }
        }
    }

    private void ResetQTEProgress()
    {
        StartCoroutine(ResetAnimation());
    }

    private IEnumerator ResetAnimation()
    {
        isResetting = true;

        // Tint all buttons red to show an error
        if (qteButtonImages != null)
        {
            for (int i = 0; i < sequenceLength; i++)
            {
                if (i < qteButtonImages.Length && qteButtonImages[i] != null)
                {
                    qteButtonImages[i].color = Color.red;
                }
            }
        }
        
        // Wait a tiny moment so they see the red flash
        yield return new WaitForSecondsRealtime(0.2f);
        
        currentQteIndex = 0;
        
        // Reset scale and color back to normal
        if (qteButtonImages != null)
        {
            for (int i = 0; i < sequenceLength; i++)
            {
                if (i < qteButtonImages.Length && qteButtonImages[i] != null)
                {
                    qteButtonImages[i].color = Color.white;
                    qteButtonImages[i].transform.localScale = Vector3.one;
                }
            }
        }

        isResetting = false;
    }

    public void TriggerEnding()
    {
        if (endingTriggered) return;
        endingTriggered = true;

        StartCoroutine(EndingRoutine());
    }

    private IEnumerator EndingRoutine()
    {
        Time.timeScale = 0f;
        
        PlayerFollowMouse followMouse = FindFirstObjectByType<PlayerFollowMouse>();
        if (followMouse != null)
            followMouse.enabled = false;

        // Phase 1: Dialogue
        if (!playedDialoguesThisSession && dialogueManager != null)
        {
            dialogueManager.snailPicture.texture = dialogueManager.snailScared;
            dialogueManager.StartDialogue(preQteDialogue);
            
            // Wait until dialogue is no longer active
            while (dialogueManager.IsDialogueActive)
            {
                yield return null;
            }
        }

        // Wait a frame just to be safe
        yield return null;
        
        // Ensure game is still frozen for QTE
        Time.timeScale = 0f;
        if (followMouse != null)
            followMouse.enabled = false;

        GenerateRandomQTE();

        // Safety check: If the array is completely empty, just auto-fail it to prevent softlocks
        if (qteSequence == null || qteSequence.Length == 0)
        {
            Debug.LogError("QTE Sequence failed to generate.");
            FailQTE();
            yield break;
        }

        // Phase 2: Start QTE
        currentQteIndex = 0;
        qteTimer = timeLimit;
        
        if (qteUIPanel != null)
        {
            qteUIPanel.SetActive(true);
        }
        
        qteActive = true;
    }

    private void GenerateRandomQTE()
    {
        // Safety: If you set sequence length to 10 but only added 9 UI images in the inspector,
        // this clamps it so you don't get stuck with an invisible final button!
        if (qteButtonImages != null && sequenceLength > qteButtonImages.Length)
        {
            sequenceLength = qteButtonImages.Length;
        }

        qteSequence = new KeyCode[sequenceLength];
        KeyCode[] possibleKeys = new KeyCode[] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow };
        
        for (int i = 0; i < sequenceLength; i++)
        {
            KeyCode randomKey = possibleKeys[Random.Range(0, possibleKeys.Length)];
            qteSequence[i] = randomKey;
            
            if (qteButtonImages != null && i < qteButtonImages.Length && qteButtonImages[i] != null)
            {
                qteButtonImages[i].gameObject.SetActive(true); // Ensure it's visible
                qteButtonImages[i].transform.localScale = Vector3.zero; // Hide initially for pop-in animation
                qteButtonImages[i].color = Color.white; // Reset color
                
                if (randomKey == KeyCode.UpArrow) qteButtonImages[i].sprite = upArrowSprite;
                else if (randomKey == KeyCode.DownArrow) qteButtonImages[i].sprite = downArrowSprite;
                else if (randomKey == KeyCode.LeftArrow) qteButtonImages[i].sprite = leftArrowSprite;
                else if (randomKey == KeyCode.RightArrow) qteButtonImages[i].sprite = rightArrowSprite;
            }
        }
        
        // Hide any extra UI images if the user put too many in the array
        if (qteButtonImages != null)
        {
            for (int i = sequenceLength; i < qteButtonImages.Length; i++)
            {
                if (qteButtonImages[i] != null) qteButtonImages[i].gameObject.SetActive(false);
            }
        }

        StartCoroutine(AnimateButtonsIn());
    }

    private IEnumerator AnimateButtonsIn()
    {
        for (int i = 0; i < sequenceLength; i++)
        {
            if (qteButtonImages != null && i < qteButtonImages.Length && qteButtonImages[i] != null)
            {
                StartCoroutine(PopInButton(qteButtonImages[i]));
                yield return new WaitForSecondsRealtime(0.05f); // Cascading delay
            }
        }
    }

    private IEnumerator PopInButton(UnityEngine.UI.Image img)
    {
        float elapsed = 0f;
        float duration = 0.2f;
        img.transform.localScale = Vector3.zero;
        Vector3 overshoot = new Vector3(1.2f, 1.2f, 1f);
        
        while (elapsed < duration)
        {
            if (img == null) yield break;
            elapsed += Time.unscaledDeltaTime; // Use unscaled because timescale is 0
            float t = elapsed / duration;
            if (t < 0.5f) {
                img.transform.localScale = Vector3.Lerp(Vector3.zero, overshoot, t * 2f);
            } else {
                img.transform.localScale = Vector3.Lerp(overshoot, Vector3.one, (t - 0.5f) * 2f);
            }
            yield return null;
        }
        if (img != null) img.transform.localScale = Vector3.one;
    }

    private IEnumerator AnimateButtonPress(UnityEngine.UI.Image img)
    {
        img.color = new Color(0.5f, 0.5f, 0.5f, 1f); // Darken the image
        
        float elapsed = 0f;
        float duration = 0.1f;
        Vector3 startScale = img.transform.localScale;
        Vector3 pressedScale = new Vector3(0.7f, 0.7f, 1f); // Shrink down
        
        while (elapsed < duration)
        {
            if (img == null) yield break;
            elapsed += Time.unscaledDeltaTime;
            img.transform.localScale = Vector3.Lerp(startScale, pressedScale, elapsed / duration);
            yield return null;
        }
        if (img != null) img.transform.localScale = pressedScale;
    }

    private void SucceedQTE()
    {
        qteActive = false;
        StartCoroutine(SucceedRoutine());
    }

    private IEnumerator SucceedRoutine()
    {
        // Don't wait here! Start the bucket sequence IMMEDIATELY so the user knows they won!
        // This stops them from feeling like they have to press the last key twice.

        // Freeze the snail forever
        if (snailTransform != null)
        {
            SnailFollow snailFollow = snailTransform.GetComponent<SnailFollow>();
            if (snailFollow != null) snailFollow.isFrozen = true;
        }

        // Fade out music
        if (snailHealthBar != null && snailHealthBar.gameMusic != null)
        {
            StartCoroutine(FadeOutAudio(snailHealthBar.gameMusic, 2f));
        }

        // Spawn and animate the bucket INSTANTLY
        if (bucketPrefab != null && snailTransform != null)
        {
            // Spawn it high up and slightly forward in the Z axis so it covers the snail
            Vector3 startPos = snailTransform.position + new Vector3(0, 10f, -1f);
            Vector3 targetPos = snailTransform.position + new Vector3(0, 0, -1f);
            GameObject bucket = Instantiate(bucketPrefab, startPos, Quaternion.identity);
            StartCoroutine(AnimateBucket(bucket, targetPos));
        }
        
        Debug.Log("moving to the next stage");

        // NOW we can wait a tiny bit before hiding the UI so they see the 10th button get pressed!
        yield return new WaitForSecondsRealtime(0.2f);
        if (qteUIPanel != null) qteUIPanel.SetActive(false);
        
        // Wait for the bucket drop animation to fully finish (0.8s drop + 0.25s recover)
        // Since we already waited 0.2s, wait another 1.2s to be perfectly safe
        yield return new WaitForSecondsRealtime(1.2f);

        // Phase 3: Trapped Dialogue
        if (dialogueManager != null)
        {
            if (bucketDialoguePicture != null) dialogueManager.snailPicture.texture = bucketDialoguePicture;
            dialogueManager.StartDialogue(trappedDialogue);
            while (dialogueManager.IsDialogueActive) yield return null;
        }

        // Small dramatic pause before the explosion happens
        yield return new WaitForSecondsRealtime(1.5f);

        // Phase 4: Break the wall!
        StartCoroutine(ScreenShake(0.5f, 0.2f)); // Cool camera shake effect!
        StartCoroutine(FlashScreen());           // Dramatic explosion flash!
        
        // Remove ALL salt and stop the spawner during the flashbang!
        SaltSpawner spawner = FindFirstObjectByType<SaltSpawner>();
        if (spawner != null) spawner.gameObject.SetActive(false);
        
        GameObject[] salts = GameObject.FindGameObjectsWithTag("Salt");
        foreach(GameObject s in salts) Destroy(s);
        
        GameObject[] snailSalts = GameObject.FindGameObjectsWithTag("SnailSalt");
        foreach(GameObject s in snailSalts) Destroy(s);
        
        if (brokenWallOverlay != null)
        {
            brokenWallOverlay.SetActive(true);
        }

        // Wait a bit so the player can process the broken wall before the snail starts yelling
        yield return new WaitForSecondsRealtime(2.5f);

        // Phase 5: Snail yells as you leave
        if (dialogueManager != null)
        {
            // Snail is still trapped under the bucket!
            if (bucketDialoguePicture != null) dialogueManager.snailPicture.texture = bucketDialoguePicture;
            dialogueManager.StartDialogue(brokenWallDialogue);
            while (dialogueManager.IsDialogueActive) yield return null;
        }
        
        Debug.Log("moving to the next stage");

        // Clean up timescale but DO NOT give the player their control back!
        Time.timeScale = 1f;
        PlayerFollowMouse followMouse = FindFirstObjectByType<PlayerFollowMouse>();
        if (followMouse != null)
            followMouse.enabled = false; // POSSESSED!
            
        // Trigger the possessed dragging!
        escapePhaseActive = true;
    }

    private IEnumerator ScreenShake(float duration, float magnitude)
    {
        if (Camera.main == null) yield break;
        
        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            Camera.main.transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
    }

    private IEnumerator FlashScreen()
    {
        // Dynamically create a white overlay canvas for a "flashbang" explosion effect
        GameObject flashObj = new GameObject("ExplosionFlash");
        Canvas canvas = flashObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999; // Ensure it renders over everything
        
        UnityEngine.UI.Image img = flashObj.AddComponent<UnityEngine.UI.Image>();
        img.color = Color.white;
        
        float duration = 0.6f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float alpha = 1f - (elapsed / duration);
            img.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        
        Destroy(flashObj);
    }

    private IEnumerator FadeOutAudio(AudioSource audioSrc, float duration)
    {
        float startVol = audioSrc.volume;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            audioSrc.volume = Mathf.Lerp(startVol, 0f, elapsed / duration);
            yield return null;
        }
        audioSrc.volume = 0f;
    }

    private IEnumerator AnimateBucket(GameObject bucket, Vector3 targetPos)
    {
        float duration = 0.8f; // Slower cartoon drop
        float elapsed = 0f;
        Vector3 startPos = bucket.transform.position;
        Vector3 originalScale = bucket.transform.localScale;
        
        // The bucket stretches vertically and squashes horizontally as it picks up speed
        Vector3 stretchScale = new Vector3(originalScale.x * 0.7f, originalScale.y * 1.4f, originalScale.z);
        
        while (elapsed < duration)
        {
            if (bucket == null) yield break; // Safety check
            elapsed += Time.unscaledDeltaTime;
            
            // Adding a simple ease-in (gravity) effect by squaring the t value
            float t = elapsed / duration;
            t = t * t; 
            
            bucket.transform.position = Vector3.Lerp(startPos, targetPos, t);
            
            // As it falls faster, stretch it more
            bucket.transform.localScale = Vector3.Lerp(originalScale, stretchScale, t);
            
            yield return null;
        }
        if (bucket == null) yield break;
        bucket.transform.position = targetPos;

        // BOOM! Hit the ground, extreme squash effect!
        Vector3 squashScale = new Vector3(originalScale.x * 1.5f, originalScale.y * 0.5f, originalScale.z);
        bucket.transform.localScale = squashScale;
        
        // Quickly boing back to its original shape
        float recoverDuration = 0.25f;
        elapsed = 0f;
        while (elapsed < recoverDuration)
        {
            if (bucket == null) yield break;
            elapsed += Time.unscaledDeltaTime;
            // Use an ease-out formula to make it snap back nicely
            float t = elapsed / recoverDuration;
            float easeOut = 1f - (1f - t) * (1f - t);
            
            bucket.transform.localScale = Vector3.Lerp(squashScale, originalScale, easeOut);
            yield return null;
        }
        
        if (bucket != null) bucket.transform.localScale = originalScale;
    }

    private void FailQTE()
    {
        qteActive = false;
        if (qteUIPanel != null) qteUIPanel.SetActive(false);

        // Heal Snail to 50%
        if (snailHealthBar != null)
        {
            snailHealthBar.SetHealth((int)(snailHealthBar.slider.maxValue * 0.5f));
        }

        StartCoroutine(FailRoutine());
    }

    private IEnumerator FailRoutine()
    {
        if (!playedDialoguesThisSession && dialogueManager != null)
        {
            playedDialoguesThisSession = true;
            dialogueManager.snailPicture.texture = dialogueManager.snailNormal; // snailIdle equivalent
            dialogueManager.StartDialogue(failQteDialogue);

            while (dialogueManager.IsDialogueActive)
            {
                yield return null;
            }
        }

        endingTriggered = false; // Reset so they can try again if they beat him again
        if (snailHealthBar != null) snailHealthBar.isDying = false;

        Time.timeScale = 1f;
        PlayerFollowMouse followMouse = FindFirstObjectByType<PlayerFollowMouse>();
        if (followMouse != null)
            followMouse.enabled = true;
    }
}
