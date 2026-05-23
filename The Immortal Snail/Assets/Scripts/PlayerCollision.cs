using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public SnailFollow snail;
    public SaltSpawner spawner;
    public CurseManager curseManager;

    private bool isHoveringSalt = false;
    private GameObject currentSalt;

    void Update()
    {
        if (isHoveringSalt && Input.GetMouseButtonDown(0))
        {
            if (currentSalt.CompareTag("Salt"))
            {
                Debug.Log("Collected Salt");

                snail.FreezeSnail(2f);

                spawner.AddSaltCollected();

                Destroy(currentSalt);
            }
            else if (currentSalt.CompareTag("SnailSalt"))
            {
                Debug.Log("You picked the wrong salt!");

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
            Debug.Log("You lost!");
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