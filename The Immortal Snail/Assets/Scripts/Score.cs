using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    public PlayerCollision playerCollision;
    public TMP_Text score;

    void Start()
    {
        score.text = "00";
    }

    void Update()
    {
        score.text = playerCollision.saltCollected.ToString("00");
    }
}