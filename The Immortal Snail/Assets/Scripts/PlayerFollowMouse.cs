using UnityEngine;

public class PlayerFollowMouse : MonoBehaviour
{
    [Header("Movement Bounds")]
    public float minX = -7f;
    public float maxX = 7f;
    public float minY = -4f;
    public float maxY = 4f;

    [Header("")]
    public Sprite openHandSprite;
    public Sprite closedHandSprite;

    public bool isInverted = false;
    public bool isFrozen = false;

    private SpriteRenderer spriteRenderer;

    private Vector3 lastMouseWorldPos;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        lastMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastMouseWorldPos.z = 0f;

        // Put player sprite exactly where the mouse starts
        lastMouseWorldPos.x = Mathf.Clamp(lastMouseWorldPos.x, minX, maxX);
        lastMouseWorldPos.y = Mathf.Clamp(lastMouseWorldPos.y, minY, maxY);
        transform.position = lastMouseWorldPos;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = openHandSprite;
    }

    void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        if (isFrozen)
        {
            lastMouseWorldPos = mouseWorldPos;
            return;
        }

        Vector3 mouseDelta = mouseWorldPos - lastMouseWorldPos;

        if (isInverted)
        {
            transform.position -= mouseDelta;
        }
        else
        {
            transform.position += mouseDelta;
        }

        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minX, maxX);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minY, maxY);
        transform.position = clampedPosition;

        lastMouseWorldPos = mouseWorldPos;

        if (Input.GetMouseButton(0))
        {
            spriteRenderer.sprite = closedHandSprite;
        }
        else
        {
            spriteRenderer.sprite = openHandSprite;
        }
    }

    public void SyncWithRealMouse()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        mouseWorldPos.x = Mathf.Clamp(mouseWorldPos.x, minX, maxX);
        mouseWorldPos.y = Mathf.Clamp(mouseWorldPos.y, minY, maxY);

        transform.position = mouseWorldPos;
        lastMouseWorldPos = mouseWorldPos;
    }
}       