using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public Slider musicSlider;
    public Slider voicelinesSlider;
    public Slider soundEffectsSlider;

    [Header("FullScreen Buttons")]
    public Image offDeselectedFullScreen;
    public Image onDeselectedFullScreen;
    public Image offFullScreen;
    public Image onFullScreen;

    [Header("Audio")]
    public AudioMixer audioMixer;
    public AudioClip tickSound;
    public AudioSource audioSource;

    void Start()
    {
        if(Screen.fullScreen == true)
        {
            offDeselectedFullScreen.gameObject.SetActive(true);
            onFullScreen.gameObject.SetActive(true);
            offFullScreen.gameObject.SetActive(false);
            onDeselectedFullScreen.gameObject.SetActive(false);
        }
        else if(Screen.fullScreen == false)
        {
            offFullScreen.gameObject.SetActive(true);
            onDeselectedFullScreen.gameObject.SetActive(true);
            onFullScreen.gameObject.SetActive(false);
            offDeselectedFullScreen.gameObject.SetActive(false);
        }

        musicSlider.onValueChanged.AddListener(OnSliderMoved);
        voicelinesSlider.onValueChanged.AddListener(OnSliderMoved);
        soundEffectsSlider.onValueChanged.AddListener(OnSliderMoved);
    }

    void OnSliderMoved(float value)
    {
        audioSource.PlayOneShot(tickSound);
    }

    #region Volume Sliders
    public void SetVolume(float volume)
    {
        // normalize 0-9 to 0.0001-1, then convert to dB
        float normalized = volume / 9f;
        normalized = Mathf.Max(normalized, 0.0001f); // avoid Log10(0)
        audioMixer.SetFloat("volume", Mathf.Log10(normalized) * 20f);
    }

    public void SetVoicelineVolume(float volume)
    {
        // normalize 0-9 to 0.0001-1, then convert to dB
        float normalized = volume / 9f;
        normalized = Mathf.Max(normalized, 0.0001f); // avoid Log10(0)
        audioMixer.SetFloat("VoicelinesVolume", Mathf.Log10(normalized) * 20f);
    }

    public void SoundEffectVolume(float volume)
    {
        // normalize 0-9 to 0.0001-1, then convert to dB
        float normalized = volume / 9f;
        normalized = Mathf.Max(normalized, 0.0001f); // avoid Log10(0)
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(normalized) * 20f);
    }

    #endregion

    #region FullScreen Logic
    public void FullScreenOn()
    {
        onFullScreen.gameObject.SetActive(true);
        offFullScreen.gameObject.SetActive(false);
        onDeselectedFullScreen.gameObject.SetActive(false);
        offDeselectedFullScreen.gameObject.SetActive(true);

        Screen.fullScreen = true;
    }

    public void FullScreenOff()
    {
        onFullScreen.gameObject.SetActive(false);
        offFullScreen.gameObject.SetActive(true);
        onDeselectedFullScreen.gameObject.SetActive(true);
        offDeselectedFullScreen.gameObject.SetActive(false);

        Screen.fullScreen = false;
    }
    #endregion
}
