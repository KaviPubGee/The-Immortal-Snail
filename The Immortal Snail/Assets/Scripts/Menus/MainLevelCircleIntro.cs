using UnityEngine;

public class MainLevelCircleIntro : MonoBehaviour
{
    public RectTransform circleMask;
    public Animator animator;

    void Start()
    {
        circleMask.gameObject.SetActive(true);
        animator.Play("CircleEnlarge");
    }
}