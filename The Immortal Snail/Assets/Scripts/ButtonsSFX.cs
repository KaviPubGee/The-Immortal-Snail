using UnityEngine.EventSystems;
using UnityEngine;

public class ButtonsSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverSound;
    public AudioClip clicked;
    public AudioSource audioSource;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound == null)
        {
            return;
        }

        audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clicked == null)
        {
            return;
        }
        
        audioSource.PlayOneShot(clicked);
    }
}
