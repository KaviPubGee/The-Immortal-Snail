using UnityEngine;
using System.Collections;

public class SnailFollow : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Transform player;
    public bool isFrozen = false;

    private Animator animator;
    public SpriteRenderer spriteRenderer;

    private bool isPlayingSpecialAnimation = false;
    private bool lastFlipX = false;
    private string currentAnimation = "";

    private enum SnailDirection
    {
        Right,
        Left,
        Up,
        Down
    }

    private SnailDirection lastDirection = SnailDirection.Right;

    void Start()
    {
        animator = GetComponent<Animator>();
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

        ApplyFlipForSpecialAnimation();
        PlayAnimation(GetHideAnimation());

        yield return new WaitForSeconds(0.5f);

        isFrozen = true;

        yield return new WaitForSeconds(duration);

        isFrozen = false;

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
}