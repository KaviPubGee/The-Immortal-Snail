using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class CursePostProcessing : MonoBehaviour
{
    public Volume volume;

    private ChromaticAberration chromaticAberration;
    private Vignette vignette;

    void Start()
    {
        volume.profile.TryGet(out chromaticAberration);
        volume.profile.TryGet(out vignette);
    }

    public void PlayCurseEffect()
    {
        StartCoroutine(CurseEffectRoutine());
    }

    IEnumerator CurseEffectRoutine()
    {
        float duration = 1.0f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            
            // Sine wave to go from 0 to 1 and back to 0
            float intensity = Mathf.Sin(t * Mathf.PI);

            if (chromaticAberration != null)
                chromaticAberration.intensity.value = intensity * 0.4f;

            if (vignette != null)
                vignette.intensity.value = 0.32f + (intensity * 0.23f);

            yield return null;
        }

        if (chromaticAberration != null)
            chromaticAberration.intensity.value = 0f;

        if (vignette != null)
            vignette.intensity.value = 0.32f;
    }
}