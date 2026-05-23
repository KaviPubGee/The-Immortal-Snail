using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public SnailFollow snail;

    private bool isHoveringSalt = false;
    private GameObject currentSalt;

    void Update()
    {
        if (isHoveringSalt && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Collected Salt");

            snail.FreezeSnail(2f);

            Destroy(currentSalt);
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
            currentSalt = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Salt"))
        {
            isHoveringSalt = false;
            currentSalt = null;
        }
    }
}