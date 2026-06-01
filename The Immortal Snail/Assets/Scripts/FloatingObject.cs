using UnityEngine;

public class FloatingObject : MonoBehaviour
{
    [SerializeField] private float speed = 2f;    // How fast the object moves
    [SerializeField] private float height = 0.5f; // How far up and down it goes

    private Vector3 startPosition;

    void Start()
    {
        // Store the starting position of the object
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new Y position using a sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * height;

        // Apply the new position while keeping X and Z unchanged
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}