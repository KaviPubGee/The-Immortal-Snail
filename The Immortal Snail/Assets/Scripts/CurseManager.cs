using UnityEngine;
using System.Collections;

public class CurseManager : MonoBehaviour
{
    public enum CurseType
    {
        InvertMouse,
        InvisibleSnail,
        InvisiblePlayer,
        PlayerFrozen,
        SnailSpeedBoost,
        InstantGameOver
    }

    [Header("References")]
    public PlayerFollowMouse playerMovement;
    public SpriteRenderer playerSprite;
    public SnailFollow snail;
    public SpriteRenderer snailSprite;
    public GameOverManager gameOverManager;


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


    public void TriggerRandomCurse()
    {
        int randomNumber = Random.Range(1, 101);

        CurseType chosenCurse;

        if (randomNumber <= 25)
        {
            chosenCurse = CurseType.InvertMouse;
        }
        else if (randomNumber <= 45)
        {
            chosenCurse = CurseType.InvisibleSnail;
        }
        else if (randomNumber <= 65)
        {
            chosenCurse = CurseType.InvisiblePlayer;
        }
        else if (randomNumber <= 85)
        {
            chosenCurse = CurseType.PlayerFrozen;
        }
        else if (randomNumber <= 95)
        {
            chosenCurse = CurseType.SnailSpeedBoost;
        }
        else
        {
            chosenCurse = CurseType.InstantGameOver;
        }

        ApplyCurse(chosenCurse);
    }

    private void ApplyCurse(CurseType curse)
    {
        if (curse == CurseType.InvertMouse)
        {
            StartCoroutine(InvertMouseRoutine());
        }
        else if (curse == CurseType.InvisibleSnail)
        {
            StartCoroutine(InvisibleSnailRoutine());
        }
        else if (curse == CurseType.InvisiblePlayer)
        {
            StartCoroutine(InvisiblePlayerRoutine());
        }
        else if (curse == CurseType.PlayerFrozen)
        {
            StartCoroutine(PlayerFrozenRoutine());
        }
        else if (curse == CurseType.SnailSpeedBoost)
        {
            StartCoroutine(SnailSpeedBoostRoutine());
        }
        else if (curse == CurseType.InstantGameOver)
        {
            if (gameOverManager != null)
                gameOverManager.TriggerGameOver();
        }
    }

    private IEnumerator InvertMouseRoutine()
    {
        Debug.Log("CURSE: Inverted mouse");

        playerMovement.isInverted = true;

        yield return new WaitForSeconds(invertMouseTime);

        playerMovement.isInverted = false;

        Debug.Log("Mouse normal again");
    }

    private IEnumerator InvisibleSnailRoutine()
    {
        Debug.Log("CURSE: Snail is almost invisible");

        Color originalColor = snailSprite.color;

        Color newColor = originalColor;
        newColor.a = invisibleSnailAlpha;
        snailSprite.color = newColor;

        yield return new WaitForSeconds(invisibleSnailTime);

        snailSprite.color = originalColor;

        Debug.Log("Snail visible again");
    }

    private IEnumerator InvisiblePlayerRoutine()
    {
        Debug.Log("CURSE: Player is almost invisible");

        Color originalColor = playerSprite.color;

        Color newColor = originalColor;
        newColor.a = invisiblePlayerAlpha;
        playerSprite.color = newColor;

        yield return new WaitForSeconds(invisiblePlayerTime);

        playerSprite.color = originalColor;

        Debug.Log("Player visible again");
    }

    private IEnumerator PlayerFrozenRoutine()
    {
        Debug.Log("CURSE: Sticky salt - player frozen");

        playerMovement.isFrozen = true;

        yield return new WaitForSeconds(playerFrozenTime);

        playerMovement.isFrozen = false;

        Debug.Log("Player can move again");
    }

    private IEnumerator SnailSpeedBoostRoutine()
    {
        Debug.Log("CURSE: Snail speed boost");

        float originalSpeed = snail.moveSpeed;

        snail.moveSpeed = originalSpeed * snailBoostMultiplier;

        yield return new WaitForSeconds(snailBoostTime);

        snail.moveSpeed = originalSpeed;

        Debug.Log("Snail speed normal again");
    }
}