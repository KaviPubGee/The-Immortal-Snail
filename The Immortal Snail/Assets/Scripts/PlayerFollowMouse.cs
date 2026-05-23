using UnityEngine;

public class PlayerFollowMouse : MonoBehaviour
{
    [Header("Movement Bounds")]
    public float minX = -7f;
    public float maxX = 7f;
    public float minY = -4f;
    public float maxY = 4f;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Stop player from leaving the play area
        mousePos.x = Mathf.Clamp(mousePos.x, minX, maxX);
        mousePos.y = Mathf.Clamp(mousePos.y, minY, maxY);

        transform.position = mousePos;
    }
}