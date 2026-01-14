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

    public bool        moveBetweenPositions = true;
    public Transform[] positionMarkers;

    private static readonly Color _BOSS_BEAM_TOP_COLOR      = Color.red;
    private static readonly Color _BOSS_BEAM_BOTTOM_COLOR   = new(1, 0.5f, 0f, 0.23f);
    private static readonly Color _BOSS_BEAM_FRESNEL_COLOR  = new(1, 0.5f, 0.43f, 0.5f);
    private static readonly Color _BOSS_BEAM_SCANLINE_COLOR = Color.yellow;
    private static readonly Color _BOSS_RING_COLOR          = Color.yellow;
    private static readonly Color _BOSS_LIGHT_COLOR         = Color.red;

    public Light beamLight;
    public float beamIntensity = 25f;

    public AudioSource audioSource;
    public float       offsetSeconds;

    public SpawnLocationComponent location;

    private Renderer _renderer;
    private bool     _materialInitialized;

    private void Start()
    {
        CreateMaterialInstance();
        ResetBeam();
    }

    private void CreateMaterialInstance()
    {
        if (_materialInitialized) return;
        beamMaterial         =   new Material(beamMaterial);
        _renderer            ??= GetComponent<Renderer>();
        _renderer.material   =   beamMaterial;
        _materialInitialized =   true;
    }

    public void MoveTo(int index)
    {
        if (!moveBetweenPositions || positionMarkers.Length == 0) return;
        var targetPosition = positionMarkers[index % positionMarkers.Length].position;
        transform.DOMove(targetPosition, 2.0f).SetEase(Ease.InOutSine);
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInCoroutine());
    }

    private IEnumerator FadeInCoroutine()
    {
        yield return new WaitForSeconds(offsetSeconds);

        CreateMaterialInstance();
        audioSource?.Play();
        beamMaterial?.DOFloat(1.0f, _ALPHA, 1.0f).SetEase(Ease.InOutSine);
        beamMaterial?.DOFloat(1.0f, _SIZE,  1.0f).SetEase(Ease.InOutSine);
        beamLight.DOIntensity(beamIntensity, 1.0f).SetEase(Ease.InOutSine).SetDelay(0.2f);
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        yield return new WaitForSeconds(offsetSeconds);

        beamMaterial?.DOFloat(0.0f, _ALPHA, 1.0f).SetEase(Ease.InOutSine);
        beamMaterial?.DOFloat(0.0f, _SIZE,  1.0f).SetEase(Ease.InOutSine);
        beamLight.DOIntensity(0.0f, 1.0f).SetEase(Ease.InOutSine);
        audioSource?.Stop();

        yield return new WaitForSeconds(1.0f);
        ResetBeam();
    }

    public void FadeToBossColor()
    {
        CreateMaterialInstance();
        beamMaterial?.DOColor(_BOSS_BEAM_TOP_COLOR,      "_TopColor",      1.0f).SetEase(Ease.InOutSine);
        beamMaterial?.DOColor(_BOSS_BEAM_BOTTOM_COLOR,   "_BottomColor",   1.0f).SetEase(Ease.InOutSine).SetDelay(0.2f);
        beamMaterial?.DOColor(_BOSS_RING_COLOR,          "_RingColor",     1.0f).SetEase(Ease.InOutSine).SetDelay(0.4f);
        beamMaterial?.DOColor(_BOSS_BEAM_FRESNEL_COLOR,  "_FresnelColor",  1.0f).SetEase(Ease.InOutSine);
        beamMaterial?.DOColor(_BOSS_BEAM_SCANLINE_COLOR, "_ScanlineColor", 1.0f).SetEase(Ease.InOutSine);
        beamLight.DOColor(_BOSS_LIGHT_COLOR, 1.0f).SetEase(Ease.InOutSine);
    }

    private void ResetBeam()
    {
        beamMaterial.SetFloat(_RANDOM_OFFSET, Random.Range(0.0f, 100.0f));
        beamMaterial?.SetFloat(_ALPHA, 0.0f);
        beamMaterial?.SetFloat(_SIZE,  0.0f);
        beamLight.intensity = 0.0f;
    }
}