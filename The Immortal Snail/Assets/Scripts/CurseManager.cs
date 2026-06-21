using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.Rendering.Universal;

public class CurseManager : MonoBehaviour
{
    public enum CurseType
    {
        InvertMouse,
        InvisibleSnail,
        InvisiblePlayer,
        PlayerFrozen,
        SnailSpeedBoost,
        SnailFullHealth,
        InstantGameOver
    }

    [Header("References")]
    public PlayerFollowMouse playerMovement;
    public CursePostProcessing cursePostProcessing;
    public SpriteRenderer playerSprite;
    public SnailFollow snail;
    public SpriteRenderer snailSprite;
    public CanvasGroup snailHealthCanvasGroup; // Use this to fade the UI slider!
    public GameOverManager gameOverManager;
    public PlayerCollision playerCollision;
    public CursePostProcessing postProcessing; // Hooks up the glitch effect!

    [Header("UI")]
    public GameObject cursePanel;  // Assign the black strip panel here!
    public TMP_Text curseText; 

    [Header("Debug")]
    public bool enableDebugKeys = false; // Check this box to spawn curses with keys 1-6

    [Header("Curse Durations")]
    public float invertMouseTime = 4f;
    public float invisibleSnailTime = 4f;
    public float invisiblePlayerTime = 3f;
    public float playerFrozenTime = 2f;
    public float snailBoostTime = 4f;

    [Header("Curse Settings")]
    public float invisibleSnailAlpha = 0.1f;
    public float invisiblePlayerAlpha = 0f;
    public float snailBoostMultiplier = 2f;

    private CanvasGroup cursePanelCg;
    private CanvasGroup curseTextCg;

    void Start()
    {
        if (cursePanel != null) 
        {
            cursePanel.SetActive(false);
            cursePanelCg = cursePanel.GetComponent<CanvasGroup>();
            if (cursePanelCg == null) cursePanelCg = cursePanel.AddComponent<CanvasGroup>();
        }
        else if (curseText != null) 
        {
            curseText.gameObject.SetActive(false);
            curseTextCg = curseText.GetComponent<CanvasGroup>();
            if (curseTextCg == null) curseTextCg = curseText.gameObject.AddComponent<CanvasGroup>();
        }
    }

    void Update()
    {
        if (enableDebugKeys)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) ApplyCurse(CurseType.InvertMouse);
            if (Input.GetKeyDown(KeyCode.Alpha2)) ApplyCurse(CurseType.InvisibleSnail);
            if (Input.GetKeyDown(KeyCode.Alpha3)) ApplyCurse(CurseType.InvisiblePlayer);
            if (Input.GetKeyDown(KeyCode.Alpha4)) ApplyCurse(CurseType.PlayerFrozen);
            if (Input.GetKeyDown(KeyCode.Alpha5)) ApplyCurse(CurseType.SnailSpeedBoost);
            if (Input.GetKeyDown(KeyCode.Alpha6)) ApplyCurse(CurseType.SnailFullHealth);
            if (Input.GetKeyDown(KeyCode.Alpha7)) ApplyCurse(CurseType.InstantGameOver);
        }
    }

    public void TriggerRandomCurse()
    {
        int randomNumber = Random.Range(1, 101);

        CurseType chosenCurse;

        if (randomNumber <= 20)
            chosenCurse = CurseType.InvertMouse;
        else if (randomNumber <= 40)
            chosenCurse = CurseType.InvisibleSnail;
        else if (randomNumber <= 60)
            chosenCurse = CurseType.InvisiblePlayer;
        else if (randomNumber <= 75)
            chosenCurse = CurseType.PlayerFrozen;
        else if (randomNumber <= 85)
            chosenCurse = CurseType.SnailSpeedBoost;
        else if (randomNumber <= 95)
            chosenCurse = CurseType.SnailFullHealth;
        else
            chosenCurse = CurseType.InstantGameOver;

        if (postProcessing != null) 
            postProcessing.PlayCurseEffect();

        ApplyCurse(chosenCurse);
    }

    private void ApplyCurse(CurseType curse)
    {
        cursePostProcessing.PlayCurseEffect();

        if (curse == CurseType.InvertMouse)
            StartCoroutine(InvertMouseRoutine());
        else if (curse == CurseType.InvisibleSnail)
            StartCoroutine(InvisibleSnailRoutine());
        else if (curse == CurseType.InvisiblePlayer)
            StartCoroutine(InvisiblePlayerRoutine());
        else if (curse == CurseType.PlayerFrozen)
            StartCoroutine(PlayerFrozenRoutine());
        else if (curse == CurseType.SnailSpeedBoost)
            StartCoroutine(SnailSpeedBoostRoutine());
        else if (curse == CurseType.SnailFullHealth)
            StartCoroutine(SnailFullHealthRoutine());
        else if (curse == CurseType.InstantGameOver)
            StartCoroutine(GameOverCurseRoutine());
    }

    private IEnumerator GameOverCurseRoutine()
    {
        ShowCursePopup("INSTANT DEATH!");
        
        Time.timeScale = 0f;
        if (playerMovement != null) playerMovement.isFrozen = true;

        yield return new WaitForSecondsRealtime(2.5f);

        if (gameOverManager != null) 
            gameOverManager.TriggerGameOver();
    }

    private void ShowCursePopup(string message)
    {
        Debug.Log("CURSE: " + message);
        if (curseText != null)
        {
            curseText.text = message;

            if (cursePanel != null) cursePanel.SetActive(true);
            else curseText.gameObject.SetActive(true);

            StopCoroutine("FadeCursePopup");
            StartCoroutine("FadeCursePopup");
        }
    }

    private IEnumerator FadeCursePopup()
    {
        GameObject uiObj = cursePanel != null ? cursePanel : curseText.gameObject;
        CanvasGroup cg = cursePanel != null ? cursePanelCg : curseTextCg;

        // Fade in
        float t = 0;
        while (t < 0.3f)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = t / 0.3f;
            yield return null;
        }
        cg.alpha = 1f;

        // Hold
        yield return new WaitForSecondsRealtime(1.5f);

        // Fade out
        t = 0;
        while (t < 0.5f)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = 1f - (t / 0.5f);
            yield return null;
        }
        cg.alpha = 0f;
        uiObj.SetActive(false);
    }

    public void ForceClearUI()
    {
        StopCoroutine("FadeCursePopup");

        if (cursePanel != null) 
        {
            if (cursePanelCg != null) cursePanelCg.alpha = 0f;
            cursePanel.SetActive(false);
        }
        
        if (curseText != null) 
        {
            if (curseTextCg != null) curseTextCg.alpha = 0f;
            curseText.gameObject.SetActive(false);
        }
    }

    private IEnumerator InvertMouseRoutine()
    {
        ShowCursePopup("CONTROLS INVERTED!");
        playerMovement.isInverted = true;
        yield return new WaitForSeconds(invertMouseTime);
        playerMovement.isInverted = false;
    }

    private IEnumerator InvisibleSnailRoutine()
    {
        ShowCursePopup("INVISIBLE SNAIL!");
        
        Color newColor = snailSprite.color;
        newColor.a = invisibleSnailAlpha;
        snailSprite.color = newColor;

        if (snailHealthCanvasGroup != null)
            snailHealthCanvasGroup.alpha = invisibleSnailAlpha;

        yield return new WaitForSeconds(invisibleSnailTime);

        newColor.a = 1f;
        snailSprite.color = newColor;

        if (snailHealthCanvasGroup != null)
            snailHealthCanvasGroup.alpha = 1f;
    }

    private IEnumerator InvisiblePlayerRoutine()
    {
        ShowCursePopup("INVISIBLE PLAYER!");
        
        Color newColor = playerSprite.color;
        newColor.a = invisiblePlayerAlpha;
        playerSprite.color = newColor;

        yield return new WaitForSeconds(invisiblePlayerTime);

        newColor.a = 1f;
        playerSprite.color = newColor;
    }

    private IEnumerator PlayerFrozenRoutine()
    {
        ShowCursePopup("PLAYER FROZEN!");
        playerMovement.isFrozen = true;
        yield return new WaitForSeconds(playerFrozenTime);
        playerMovement.isFrozen = false;
    }

    private IEnumerator SnailSpeedBoostRoutine()
    {
        ShowCursePopup("SNAIL SPEED BOOST!");
        float originalSpeed = snail.moveSpeed;
        snail.moveSpeed = originalSpeed * snailBoostMultiplier;
        yield return new WaitForSeconds(snailBoostTime);
        snail.moveSpeed = originalSpeed;
    }

    private IEnumerator SnailFullHealthRoutine()
    {
        ShowCursePopup("SNAIL HEALED!");
        if (playerCollision != null)
        {
            playerCollision.GetHeal(50); // Heal back to max
        }
        yield return null;
    }
}