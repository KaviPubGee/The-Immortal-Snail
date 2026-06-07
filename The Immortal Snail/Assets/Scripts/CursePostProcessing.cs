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
        if (chromaticAberration != null)
            chromaticAberration.intensity.value = 0.25f;

        if (vignette != null)
            vignette.intensity.value = 0.55f;

        yield return new WaitForSecondsRealtime(0.4f);

        if (chromaticAberration != null)
            chromaticAberration.intensity.value = 0f;

        if (vignette != null)
            vignette.intensity.value = 0.32f;
    }
}