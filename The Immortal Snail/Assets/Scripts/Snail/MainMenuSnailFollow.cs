using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

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

    [Header("Click Feedback")]
    public ParticleSystem clickParticles;
    public float flashTime = 0.08f;
    public Color clickFlashColor = Color.gray;

    private Color originalColor;

    public PlayerFollowMouse playerFollowMouse;

    [Header("Secret Clicks")]
    public int clicks = 20;
    private int playerClicks;

    private string currentAnimation = "";

    private bool isHidden = false;
    private bool isPlayingSpecialAnimation = false;
    private bool isEscaping = false;

    private Coroutine specialRoutine;

    private int patrolDirection = 1;

    private bool mouseIsOverSnail = false;

    private bool quitButtonHoveredBySnail = false;


    void Start()
    {
        originalColor = spriteRenderer.color;
    }

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

        //Decide which adge the snail is currently walking towrds
        float targetX;

        if (patrolDirection == 1)
        {
            targetX = maxX;
        }
        else
        {
            targetX = minX;
        }

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
            patrolDirection *= -1;

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
        mouseIsOverSnail = true;

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
        mouseIsOverSnail = false;

        if (isEscaping) return;
        if (!isHidden || isPlayingSpecialAnimation)
        {
            if (specialRoutine != null)
            {
                StopCoroutine(specialRoutine);
            }
        }        

        specialRoutine = StartCoroutine(AppearRoutine());
    }

    private void OnMouseDown()
    {
        if (!isHidden) return;
        if (isEscaping) return;

        playerClicks++;

        StartCoroutine(ClickFeedback());

        Debug.Log("Hidden snail clicked: " + playerClicks);

        if (playerClicks >= clicks)
        {
            StartCoroutine(SnailEscapeRoutine());
        }
    }

    void CheckQuitButtonHover()
    {
        if (quitButton == null || quitButtonHoveredBySnail) return;

        RectTransform quitRect = quitButton.GetComponent<RectTransform>();

        Vector2 snailScreenPos = Camera.main.WorldToScreenPoint(transform.position);

        bool snailIsOverButton = RectTransformUtility.RectangleContainsScreenPoint(
            quitRect,
            snailScreenPos,
            null
        );

        if (snailIsOverButton)
        {
            quitButtonHoveredBySnail = true;

            PointerEventData pointerData = new PointerEventData(EventSystem.current);

            ExecuteEvents.Execute(
                quitButton.gameObject,
                pointerData,
                ExecuteEvents.pointerEnterHandler
            );

            Debug.Log("Snail is hovering over quit button");
        }
    }

    IEnumerator HideRoutine()
    {
        isPlayingSpecialAnimation = true;

        ForcePlayAnimation("Snail_Hide");

        yield return new WaitForSeconds(hideTime);

        isHidden = true;
        isPlayingSpecialAnimation = false;

        if (!mouseIsOverSnail && !isEscaping)
        {
            if (specialRoutine != null)
            {
                StopCoroutine(specialRoutine);
            }

            specialRoutine = StartCoroutine(AppearRoutine());
        }
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
        playerFollowMouse.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;

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

        Debug.Log("Snail reached quit button");

        if (quitButton != null)
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);

            ExecuteEvents.Execute(
                quitButton.gameObject,
                pointerData,
                ExecuteEvents.pointerEnterHandler
            );

            yield return new WaitForSeconds(0.5f);

            Debug.Log("Clicked quit button");
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

            CheckQuitButtonHover();

            yield return null;
        }

        transform.position = targetPosition;
    }

    IEnumerator ClickFeedback()
    {
        // particles
        if (clickParticles != null)
        {
            clickParticles.Play();
        }

        // tiny shake
        Vector3 originalPosition = transform.position;

        transform.position = originalPosition + new Vector3(0.03f, 0f, 0f);
        spriteRenderer.color = clickFlashColor;

        yield return new WaitForSeconds(0.04f);

        transform.position = originalPosition + new Vector3(-0.03f, 0f, 0f);

        yield return new WaitForSeconds(0.04f);

        transform.position = originalPosition;
        spriteRenderer.color = originalColor;
    }
}