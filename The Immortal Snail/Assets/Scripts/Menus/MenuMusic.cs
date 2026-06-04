using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public static MenuMusic instance;

    void Awake()
    {
        // If another menu music already exists, destroy this duplicate
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Keep this music alive between menu scenes
        DontDestroyOnLoad(gameObject);
    }

    public void StopMusic()
    {
        Destroy(gameObject);
    }
}