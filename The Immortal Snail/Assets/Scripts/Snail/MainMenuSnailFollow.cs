using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuSnailFollow : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Label Edge Limits")]
    public float minX = -4f;
    public float maxX = 4f;

    [Header("Fixed Y Position")]
    public float labelY = 2f;

    [Header("Quit Button Target")]
    public Transform quitButtonTarget;
    public Button quitButton;

    [Header("Animation")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("Special Animation Times")]
    public float hideTime = 0.5f;
    public float appearTime = 0.5f;

    [Header("Secret Clicks")]
    public int clicks = 20;
    private int playerClicks;

    private string currentAnimation = "";

    private bool isHidden = false;
    private bool isPlayingSpecialAnimation = false;
    private bool isEscaping = false;

    private Coroutine specialRoutine;

    void Update()
    {
        if (isEscaping)
        {
            return;
        }

        if (isHidden || isPlayingSpecialAnimation)
        {
            return;
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        float targetX = Mathf.Clamp(mouseWorldPos.x, minX, maxX);

        Vector3 targetPosition = new Vector3(targetX, labelY, transform.position.z);

        float distance = Mathf.Abs(transform.position.x - targetX);

        if (distance > 0.02f)
        {
            Vector3 oldPosition = transform.position;

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            float moveDirection = transform.position.x - oldPosition.x;

            if (moveDirection > 0)
            {
                spriteRenderer.flipX = false;
            }
            else if (moveDirection < 0)
            {
                spriteRenderer.flipX = true;
            }

            PlayAnimation("Snail_Crawl_Right");
        }
        else
        {
            animator.speed = 0f;
            currentAnimation = "";
        }
    }

    void PlayAnimation(string animationName)
    {
        animator.speed = 1f;

        if (currentAnimation == animationName) return;

        animator.Play(animationName);
        currentAnimation = animationName;
    }

    void ForcePlayAnimation(string animationName)
    {
        animator.speed = 1f;
        animator.Play(animationName, 0, 0f);
        currentAnimation = animationName;
    }

    private void OnMouseEnter()
    {
        if (isEscaping) return;
        if (isHidden || isPlayingSpecialAnimation) return;

        if (specialRoutine != null)
        {
            StopCoroutine(specialRoutine);
        }

        specialRoutine = StartCoroutine(HideRoutine());
    }

    private void OnMouseExit()
    {
        if (isEscaping) return;
        if (!isHidden || isPlayingSpecialAnimation) return;

        if (specialRoutine != null)
        {
            StopCoroutine(specialRoutine);
        }

        specialRoutine = StartCoroutine(AppearRoutine());
    }

    private void OnMouseDown()
    {
        if (!isHidden) return;
        if (isEscaping) return;

        playerClicks++;

        Debug.Log("Hidden snail clicked: " + playerClicks);

        if (playerClicks >= clicks)
        {
            StartCoroutine(SnailEscapeRoutine());
        }
    }

    IEnumerator HideRoutine()
    {
        isPlayingSpecialAnimation = true;

        ForcePlayAnimation("Snail_Hide");

        yield return new WaitForSeconds(hideTime);

        isHidden = true;
        isPlayingSpecialAnimation = false;
    }

    IEnumerator AppearRoutine()
    {
        isPlayingSpecialAnimation = true;

        ForcePlayAnimation("Snail_Appear");

        yield return new WaitForSeconds(appearTime);

        isHidden = false;
        isPlayingSpecialAnimation = false;
        currentAnimation = "";

        animator.speed = 0f;
    }

    IEnumerator SnailEscapeRoutine()
    {
        isEscaping = true;
        isHidden = false;
        isPlayingSpecialAnimation = true;

        // First come out of shell
        ForcePlayAnimation("Snail_Appear");
        yield return new WaitForSeconds(appearTime);

        isPlayingSpecialAnimation = false;

        // Decide which title edge to run to
        float middleX = (minX + maxX) / 2f;
        float edgeX;

        if (transform.position.x >= middleX)
        {
            edgeX = maxX;
        }
        else
        {
            edgeX = minX;
        }

        // 1. Move to title edge
        Vector3 edgePosition = new Vector3(edgeX, labelY, transform.position.z);
        PlayAnimation("Snail_Crawl_Right");
        yield return StartCoroutine(MoveToPosition(edgePosition));

        // 2. Go down beside the title toward the quit button Y
        Vector3 downPosition = new Vector3(edgeX, quitButtonTarget.position.y, transform.position.z);

        // If you have a down crawl animation, use it here
        PlayAnimation("Snail_Crawl_Down");
        yield return StartCoroutine(MoveToPosition(downPosition));

        // 3. Turn toward the quit button
        Vector3 quitPosition = new Vector3(quitButtonTarget.position.x, quitButtonTarget.position.y, transform.position.z);

        if (quitPosition.x > transform.position.x)
        {
            spriteRenderer.flipX = false; // face right
        }
        else
        {
            spriteRenderer.flipX = true; // face left
        }

        PlayAnimation("Snail_Crawl_Right");
        yield return StartCoroutine(MoveToPosition(quitPosition));

        animator.speed = 0f;

        Debug.Log("Snail clicked quit button");

        if (quitButton != null)
        {
            quitButton.onClick.Invoke();
        }
    }

    IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.03f)
        {
            Vector3 oldPosition = transform.position;

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            float moveDirection = transform.position.x - oldPosition.x;

            if (Mathf.Abs(moveDirection) > 0.001f)
            {
                if (moveDirection > 0)
                {
                    spriteRenderer.flipX = false;
                }
                else
                {
                    spriteRenderer.flipX = true;
                }
            }

            yield return null;
        }

        transform.position = targetPosition;
    }
}