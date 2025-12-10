using UnityEngine;

public class BulletRainbowComponent : MonoBehaviour
{
    private static readonly int _EMISSION_COLOR = Shader.PropertyToID("_EmissionColor");

    public  float    colorChangeSpeed = 2f;
    public  Renderer _renderer;
    private float    _hue;

    public float minAlpha = 0.5f;
    public float maxAlpha = 1f;

    private void Update()
    {
        _hue += colorChangeSpeed * Time.deltaTime;
        if (_hue > 1f)
            _hue -= 1f;

        var color = Color.HSVToRGB(_hue, 1f, 1f);
        color.a = Mathf.Lerp(minAlpha, maxAlpha, Mathf.PingPong(Time.time * colorChangeSpeed, 1f));

        if (_renderer == null) return;
        
        _renderer.material.color = color;
        _renderer.material.SetColor(_EMISSION_COLOR, color);
    }
}