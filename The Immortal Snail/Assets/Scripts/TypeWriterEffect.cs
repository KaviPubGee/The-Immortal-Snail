using UnityEngine;
using System.Collections;
using TMPro;

public class TypeWriterEffect : MonoBehaviour
{
    public bool isTyping = false;

    private Coroutine typingCoroutine;
    private string currentText;
    private TMP_Text currentTextLabel;
    private float typeSpeed;

    public void Run(string textToType, TMP_Text textLabel, float speed)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        currentText = textToType;
        currentTextLabel = textLabel;
        typeSpeed = speed;

        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        currentTextLabel.text = "";

        foreach (char letter in currentText)
        {
            currentTextLabel.text += letter;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }

        currentTextLabel.text = currentText;
        isTyping = false;
    }

    public void Skip()
    {
        if (!isTyping) return;

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        currentTextLabel.text = currentText;
        isTyping = false;
    }
}