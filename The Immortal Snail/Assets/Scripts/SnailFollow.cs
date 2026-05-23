using UnityEngine;
using System.Collections;

public class SnailFollow : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Transform player;
    public bool isFrozen = false;

    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isPlayingSpecialAnimation = false;
    private bool lastFlipX = false;
    private string currentAnimation = "";

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isFrozen || isPlayingSpecialAnimation)
        {
            return;
        }

        Vector2 currentPosition = transform.position;
        Vector2 targetPosition = player.position;
        Vector2 direction = targetPosition - currentPosition;

        if (direction.magnitude > 0.01f)
        {
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
                }
                else
                {
                    spriteRenderer.flipX = true;
                    lastFlipX = true;
                }
            }
            else if (direction.y > 0)
            {
                PlayAnimation("Snail_Crawl_Up");
            }
            else if (direction.y < 0)
            {
                PlayAnimation("Snail_Crawl_Down");
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

        spriteRenderer.flipX = lastFlipX;
        PlayAnimation("Snail_Hide");

        yield return new WaitForSeconds(0.5f);

        isFrozen = true;

        yield return new WaitForSeconds(duration);

        isFrozen = false;

        spriteRenderer.flipX = lastFlipX;
        PlayAnimation("Snail_Appear");

        yield return new WaitForSeconds(0.5f);

        isPlayingSpecialAnimation = false;
        currentAnimation = "";
    }
}