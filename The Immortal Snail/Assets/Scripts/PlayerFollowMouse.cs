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
        lastMouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        lastMouseWorldPos.z = 0f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

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
}       