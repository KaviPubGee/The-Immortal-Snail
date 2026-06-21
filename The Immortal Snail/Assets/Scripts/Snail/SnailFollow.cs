using UnityEngine;
using System.Collections;

public class SnailFollow : MonoBehaviour
{
    [Header("Speed Settings")]
    public float moveSpeed = 1f;
    public float maxMoveSpeed = 3.0f; // Cap so he doesn't become Sonic
    public float speedIncreaseAmount = 0.15f; // How much speed he gains every 10s

    [Header("Hit Feedback")]
    public ParticleSystem hitParticles; // Drag a Unity Particle System here!
    public Color hitColor = Color.red;
    public float flashDuration = 0.2f;

    public Transform player;
    public bool isFrozen = false;

    [Header("Audio")]
    public AudioSource moveAudioSource; // Place your looping slither sound here
    public AudioSource sfxAudioSource;  // Place an AudioSource for snappy sound effects
    public AudioClip retractSound;
    public AudioClip appearSound;
    public AudioClip hitSound;

    private Animator animator;
    public SpriteRenderer spriteRenderer;

    private bool isPlayingSpecialAnimation = false;
    [HideInInspector]public bool lastFlipX = false;
    private string currentAnimation = "";

    private enum SnailDirection
    {
        Right,
        Left,
        Up,
        Down
    }

    private SnailDirection lastDirection = SnailDirection.Right;
    private PlayerCollision cachedPlayerCol;

    void Start()
    {
        animator = GetComponent<Animator>();
        cachedPlayerCol = FindFirstObjectByType<PlayerCollision>();
        StartCoroutine(SpeedUpOverTime());
    }

    IEnumerator SpeedUpOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            
            if (moveSpeed < maxMoveSpeed)
            {
                moveSpeed = Mathf.Min(moveSpeed + speedIncreaseAmount, maxMoveSpeed);
                Debug.Log("The snail is accelerating! Current Speed: " + moveSpeed);
            }
        }
    }

    void Update()
    {
        if (isFrozen || isPlayingSpecialAnimation || Time.timeScale == 0f)
        {
            if (moveAudioSource != null && moveAudioSource.isPlaying)
                moveAudioSource.Pause();

            return;
        }

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = player.position;
        Vector2 direction = targetPosition - currentPosition;

        if (direction.magnitude > 0.01f)
        {
            if (moveAudioSource != null && !moveAudioSource.isPlaying)
                moveAudioSource.Play();

            direction.Normalize();

            transform.position = Vector2.MoveTowards(
                currentPosition,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                PlayAnimation("Snail_Crawl_Right");

                if (direction.x > 0)
                {
                    spriteRenderer.flipX = false;
                    lastFlipX = false;
                    lastDirection = SnailDirection.Right;
                }
                else
                {
                    spriteRenderer.flipX = true;
                    lastFlipX = true;
                    lastDirection = SnailDirection.Left;
                }
            }
            else if (direction.y > 0)
            {
                PlayAnimation("Snail_Crawl_Up");
                lastDirection = SnailDirection.Up;
            }
            else if (direction.y < 0)
            {
                PlayAnimation("Snail_Crawl_Down");
                lastDirection = SnailDirection.Down;
            }
        }
    }

    void PlayAnimation(string animationName)
    {
        if (currentAnimation == animationName) return;

        animator.Play(animationName);
        currentAnimation = animationName;
    }

    public void FreezeSnail(float duration)
    {
        if (!isFrozen && !isPlayingSpecialAnimation)
        {
            StartCoroutine(FreezeRoutine(duration));
        }
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        isPlayingSpecialAnimation = true;

        if (sfxAudioSource != null && retractSound != null)
            sfxAudioSource.PlayOneShot(retractSound);

        ApplyFlipForSpecialAnimation();
        PlayAnimation(GetHideAnimation());

        yield return new WaitForSeconds(0.5f);

        isFrozen = true;

        yield return new WaitForSeconds(duration);

        isFrozen = false;

        if (sfxAudioSource != null && appearSound != null)
            sfxAudioSource.PlayOneShot(appearSound);

        ApplyFlipForSpecialAnimation();
        PlayAnimation(GetAppearAnimation());

        yield return new WaitForSeconds(0.5f);

        isPlayingSpecialAnimation = false;
        currentAnimation = "";
    }

    string GetHideAnimation()
    {
        switch (lastDirection)
        {
            case SnailDirection.Up:
                return "Snail_Hide_Up";

            case SnailDirection.Down:
                return "Snail_Hide_Down";

            case SnailDirection.Left:
            case SnailDirection.Right:
            default:
                return "Snail_Hide";
        }
    }

    string GetAppearAnimation()
    {
        switch (lastDirection)
        {
            case SnailDirection.Up:
                return "Snail_Appear_Up";

            case SnailDirection.Down:
                return "Snail_Appear_Down";

            case SnailDirection.Left:
            case SnailDirection.Right:
            default:
                return "Snail_Appear";
        }
    }

    void ApplyFlipForSpecialAnimation()
    {
        if (lastDirection == SnailDirection.Left)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlacedSalt"))
        {
            Destroy(other.gameObject);
            
            if (sfxAudioSource != null && hitSound != null)
                sfxAudioSource.PlayOneShot(hitSound);

            if (hitParticles != null) 
                hitParticles.Play();
            
            StartCoroutine(DamageFlashRoutine());

            if (cachedPlayerCol != null)
            {
                cachedPlayerCol.TakeDamage(3);
                cachedPlayerCol.snailHitsWithSalt++;

                if (cachedPlayerCol.snailHitsWithSalt >= 5 && cachedPlayerCol.spawner != null && cachedPlayerCol.spawner.snailSaltUnlocked == false)
                {
                    cachedPlayerCol.spawner.snailSaltUnlocked = true;
                    Debug.Log("The snail is evolving");
                }
            }

            FreezeSnail(2f);
        }
    }

    private bool isFlashing = false;

    private IEnumerator DamageFlashRoutine()
    {
        if (isFlashing) yield break;
        isFlashing = true;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = hitColor;
        
        yield return new WaitForSeconds(flashDuration);
        
        // Don't accidentally override the invisible curse if it happens to overlap!
        if (spriteRenderer.color == hitColor) 
        {
            spriteRenderer.color = originalColor;
        }

        isFlashing = false;
    }
}