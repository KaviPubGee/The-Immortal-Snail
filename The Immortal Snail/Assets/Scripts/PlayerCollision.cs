using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public SnailFollow snail;
    public SaltSpawner spawner;
    public CurseManager curseManager;
    public GameOverManager gameOverManager;

    private bool isHoveringSalt = false;
    private GameObject currentSalt;

    public bool pickedUpCursedSaltFirstTime = false;
    
    public int saltCollected = 0;

    void Update()
    {
        if (isHoveringSalt && Input.GetMouseButtonDown(0))
        {
            if (currentSalt.CompareTag("Salt"))
            {
                Debug.Log("Collected Salt");

                saltCollected++;

                snail.FreezeSnail(2f);

                spawner.AddSaltCollected();

                Destroy(currentSalt);
            }
            else if (currentSalt.CompareTag("SnailSalt"))
            {
                Debug.Log("You picked the wrong salt!");

                saltCollected = Mathf.Max(0, saltCollected - 1);

                pickedUpCursedSaltFirstTime = true;

                curseManager.TriggerRandomCurse();

                Destroy(currentSalt);
            }

            isHoveringSalt = false;
            currentSalt = null;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Snail"))
        {
            if (gameOverManager != null)
                gameOverManager.TriggerGameOver();
        }

        if (other.CompareTag("Salt") || other.CompareTag("SnailSalt"))
        {
            isHoveringSalt = true;
            currentSalt = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Salt") || other.CompareTag("SnailSalt"))
        {
            isHoveringSalt = false;
            currentSalt = null;
        }
    }
}