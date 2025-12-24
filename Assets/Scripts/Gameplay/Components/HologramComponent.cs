using DG.Tweening;
using UnityEngine;

public class HologramComponent : MonoBehaviour
{
    private static readonly int _EXTERNAL_GLITCH_ACTIVE = Shader.PropertyToID("_ExternalGlitchActive");

    [Header("References")]
    public Material hologramMaterial;
    public AudioSource audioSource;

    [Header("Glitch Settings")]
    public float glitchFrequency = 2.0f;
    [Range(0f, 1f)] public float glitchProbability = 0.1f;
    [Range(0f, 1f)] public float glitchDuration = 0.2f;

    private float _timer;
    private bool _isGlitching;
    private float _glitchEndTime;
    
    private bool _stopGlitching;

    private void Start()
    {
        if (hologramMaterial == null)
        {
            var rend = GetComponent<Renderer>();
            if (rend != null)
            {
                hologramMaterial = rend.material;
            }
        }
        
        if (hologramMaterial != null)
        {
            hologramMaterial.SetFloat(_EXTERNAL_GLITCH_ACTIVE, 0.0f);
        }
    }

    private void Update()
    {
        if (hologramMaterial == null || _stopGlitching) return;

        if (!_isGlitching)
        {
            _timer += Time.deltaTime;

            if (!(_timer >= (1.0f / glitchFrequency))) return;
            
            _timer = 0f;
            if (Random.value <= glitchProbability)
            {
                StartGlitch();
            }
        }
        else
        {
            if (Time.time >= _glitchEndTime)
            {
                StopGlitch();
            }
        }
    }

    private void StartGlitch()
    {
        _isGlitching = true;
        _glitchEndTime = Time.time + glitchDuration;

        hologramMaterial.SetFloat(_EXTERNAL_GLITCH_ACTIVE, 1.0f);

        if (audioSource is null) return;
        
        audioSource.pitch = Random.Range(0.95f, 1.05f);
        audioSource.Play();
    }

    private void StopGlitch()
    {
        _isGlitching = false;
        
        hologramMaterial.SetFloat(_EXTERNAL_GLITCH_ACTIVE, 0.0f);
    }
}

