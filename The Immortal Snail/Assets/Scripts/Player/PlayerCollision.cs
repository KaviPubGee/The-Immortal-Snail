using UnityEngine;
using System.Collections.Generic;

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

    [Header("Salt Grenade Logic")]
    public int saltCharges = 0;
    public GameObject thrownSaltPrefab;
    public GameObject placedSaltPrefab;
    public GameObject aimDotPrefab;       // Small circle sprite for the arc dots
    public int dotCount = 12;             // How many dots in the arc preview
    public float throwPowerMultiplier = 1.5f;
    public float arcHeight = 2f;          // Must match ThrowSalt.arcHeight!

    [Header("Audio")]
    public AudioSource playerAudio;
    public AudioClip collectSaltSound;

    [HideInInspector] public int snailHitsWithSalt = 0;

    private Vector2 slingshotStartMousePos;
    private bool isAiming = false;
    private List<GameObject> arcDots = new List<GameObject>();

    private Camera mainCamera;
    private PlayerFollowMouse followMouse;

    void Start()
    {
        mainCamera = Camera.main;
        followMouse = GetComponent<PlayerFollowMouse>();

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    void Update()
    {
        // ------------- AIMING THE SLINGSHOT -------------

        if (Input.GetMouseButtonDown(1) && saltCharges > 0)
        {
            isAiming = true;
            slingshotStartMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(1) && isAiming)
        {
            Vector2 currentMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = (slingshotStartMousePos - currentMousePos) * throwPowerMultiplier;
            Vector2 startPos = transform.position;
            Vector2 targetPos = startPos + dragVector;

            DrawArcDots(startPos, targetPos);
        }

        if (Input.GetMouseButtonUp(1) && isAiming)
        {
            isAiming = false;
            ClearArcDots();

            Vector2 finalMousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dragVector = (slingshotStartMousePos - finalMousePos) * throwPowerMultiplier;
            Vector2 targetLandingSpot = (Vector2)transform.position + dragVector;

            GameObject grenade = Instantiate(thrownSaltPrefab, transform.position, Quaternion.identity);
            ThrowSalt throwScript = grenade.GetComponent<ThrowSalt>();
            
            float distance = Vector2.Distance(transform.position, targetLandingSpot);
            throwScript.arcHeight = Mathf.Min(distance * 0.4f, arcHeight);
            
            throwScript.Toss(transform.position, targetLandingSpot, placedSaltPrefab);

            saltCharges--;
        }

        // ------------- COLLECTING SALT -------------
        if (isHoveringSalt && Input.GetMouseButtonDown(0))
        {
            if (currentSalt.CompareTag("Salt"))
            {
                Debug.Log("Collected Salt");

                if (playerAudio != null && collectSaltSound != null)
                    playerAudio.PlayOneShot(collectSaltSound);

                if (saltCharges < 1) 
                    saltCharges++;

                saltCollected++;
                spawner.AddSaltCollected();
                
                Salt saltComp = currentSalt.GetComponent<Salt>();
                if (saltComp != null) saltComp.Collect();
                else Destroy(currentSalt);
            }
            else if (currentSalt.CompareTag("SnailSalt"))
            {
                Debug.Log("You picked the wrong salt!");

                saltCollected = Mathf.Max(0, saltCollected - 1);
                pickedUpCursedSaltFirstTime = true;
                curseManager.TriggerRandomCurse();
                GetHeal(5);
                
                Salt saltComp = currentSalt.GetComponent<Salt>();
                if (saltComp != null) saltComp.Collect();
                else Destroy(currentSalt);
            }

            isHoveringSalt = false;
            currentSalt = null;
        }
    }

    void DrawArcDots(Vector2 startPos, Vector2 targetPos)
    {
        ClearArcDots();

        float distance = Vector2.Distance(startPos, targetPos);
        float dynamicHeight = Mathf.Min(distance * 0.4f, arcHeight);

        for (int i = 1; i <= dotCount; i++)
        {
            float t = (float)i / (dotCount + 1);
            Vector2 flatPos = Vector2.Lerp(startPos, targetPos, t);
            float height = Mathf.Sin(t * Mathf.PI) * dynamicHeight;
            Vector3 dotPos = new Vector3(flatPos.x, flatPos.y + height, 0);

            if (aimDotPrefab != null)
            {
                GameObject dot = Instantiate(aimDotPrefab, dotPos, Quaternion.identity);
                arcDots.Add(dot);
            }
        }
    }

    void ClearArcDots()
    {
        foreach (GameObject dot in arcDots)
        {
            if (dot != null) Destroy(dot);
        }
        arcDots.Clear();
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
        if ((other.CompareTag("Salt") || other.CompareTag("SnailSalt")) && other.gameObject == currentSalt)
        {
            isHoveringSalt = false;
            currentSalt = null;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }

    public void GetHeal(int health)
    {
        currentHealth = Mathf.Min(currentHealth + health, maxHealth);
        healthBar.SetHealth(currentHealth);
    }
}