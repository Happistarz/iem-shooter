using System.Collections;
using DG.Tweening;
using UnityEngine;

public class TractorBeamComponent : MonoBehaviour
{
    private static readonly int _ALPHA         = Shader.PropertyToID("_GlobalAlpha");
    private static readonly int _SIZE          = Shader.PropertyToID("_BottomWidth");
    private static readonly int _RANDOM_OFFSET = Shader.PropertyToID("_RandomOffset");

    [Header("References")]
    public Material beamMaterial;

    public Light beamLight;
    public float beamIntensity = 25f;

    public AudioSource audioSource;
    public float       offsetSeconds;

    private void Start()
    {
        var rend = GetComponent<Renderer>();
        if (rend != null)
        {
            if (beamMaterial != null)
            {
                rend.material = beamMaterial;
            }

            beamMaterial = rend.material;
        }

        beamMaterial.SetFloat(_RANDOM_OFFSET, Random.Range(0.0f, 100.0f));
        beamMaterial?.SetFloat(_ALPHA, 0.0f);
        beamMaterial?.SetFloat(_SIZE,  0.0f);
        beamLight.intensity = 0.0f;
        StartCoroutine(Initialize());
    }

    private IEnumerator Initialize()
    {
        yield return new WaitForSeconds(offsetSeconds);

        audioSource?.Play();
        FadeIn();
    }

    public void FadeIn()
    {
        beamMaterial?.DOFloat(1.0f, _ALPHA, 1.0f).SetEase(Ease.InOutSine);
        beamMaterial?.DOFloat(1.0f, _SIZE,  1.0f).SetEase(Ease.InOutSine);
        beamLight.DOIntensity(beamIntensity, 1.0f).SetEase(Ease.InOutSine).SetDelay(0.2f);
    }

    public void FadeOut()
    {
        beamMaterial?.DOFloat(0.0f, _ALPHA, 1.0f).SetEase(Ease.InOutSine);
        beamMaterial?.DOFloat(0.0f, _SIZE,  1.0f).SetEase(Ease.InOutSine);
        beamLight.DOIntensity(0.0f, 1.0f).SetEase(Ease.InOutSine);
    }
}