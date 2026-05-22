using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private bool isHoveringSalt = false;

    void Update()
    {
        if (isHoveringSalt && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Collected Salt");
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Snail"))
        {
            Debug.Log("You lost!");
        }

        if (other.CompareTag("Salt"))
        {
            isHoveringSalt = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Salt"))
        {
            isHoveringSalt = false;
        }
    }
}