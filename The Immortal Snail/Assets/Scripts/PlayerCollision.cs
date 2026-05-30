using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public int maxHealth = 50;
    public int currentHealth;

    public HealthBar healthBar;

    public SnailFollow snail;
    public SaltSpawner spawner;
    public CurseManager curseManager;
    public GameOverManager gameOverManager;

    private bool isHoveringSalt = false;
    private GameObject currentSalt;

    public bool pickedUpCursedSaltFirstTime = false;
    
    public int saltCollected = 0;


    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        if (isHoveringSalt && Input.GetMouseButtonDown(0))
        {
            if (currentSalt.CompareTag("Salt"))
            {
                Debug.Log("Collected Salt");

                saltCollected++;

                TakeDamage(3);

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

                GetHeal(5);

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

    void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    void GetHeal(int health)
    {
        currentHealth += health;
        healthBar.SetHealth(currentHealth);
    }
}