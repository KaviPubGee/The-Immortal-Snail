using UnityEngine;

public class SnailFollow : MonoBehaviour
{
    public float moveSpeed = 1f;
    public Transform player;
    public bool isFrozen = false;

    
    void Update()
    {
        if (isFrozen) return;

        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    public void FreezeSnail(float duration)
    {
        StartCoroutine(FreezeRoutine(duration));
    }

    private System.Collections.IEnumerator FreezeRoutine(float duration)
    {
        isFrozen = true;
        yield return new WaitForSeconds(duration);
        isFrozen = false;
    }
}
