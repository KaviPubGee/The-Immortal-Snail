using UnityEngine;
using System.Collections;
using TMPro;

public class TypeWriterEffect : MonoBehaviour
{
    public bool isTyping = false;

    public PauseMenu pauseMenu;

    private Coroutine typingCoroutine;
    private string currentText;
    private TMP_Text currentTextLabel;
    private float typeSpeed;
    private int currentCharIndex = 0;

    public void Run(string textToType, TMP_Text textLabel, float speed)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        currentText = textToType;
        currentTextLabel = textLabel;
        typeSpeed = speed;
        currentCharIndex = 0;
        currentTextLabel.text = "";

        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        isTyping = true;

        while (currentCharIndex < currentText.Length)
        {
            currentTextLabel.text += currentText[currentCharIndex];
            currentCharIndex++;

            float elapsed = 0f;
            while (elapsed < typeSpeed)
            {
                yield return null;
                if (pauseMenu == null || !pauseMenu.IsPaused)
                    elapsed += Time.unscaledDeltaTime;
            }
        }

        currentTextLabel.text = currentText;
        isTyping = false;
    }

    public void Pause()
    {
        if (!isTyping) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    public void Resume()
    {
        if (!isTyping) return;

        typingCoroutine = StartCoroutine(TypeText());
    }

    public void Skip()
    {
        if (!isTyping) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        currentTextLabel.text = currentText;
        isTyping = false;
    }
}