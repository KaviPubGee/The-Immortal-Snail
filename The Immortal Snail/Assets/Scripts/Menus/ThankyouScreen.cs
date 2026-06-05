using UnityEngine;
using UnityEngine.SceneManagement;

public class ThankyouScreen : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        Time.timeScale = 1f;
        animator.Play("FadeOutSuperSlow");
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
