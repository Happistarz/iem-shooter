using UnityEngine;

public class HologramRainbowEffect : MonoBehaviour
{
    private static readonly int _MOUSE_POS_PROPERTY = Shader.PropertyToID("_MousePos");
    private static readonly int _RAINBOW_INTENSITY_PROPERTY = Shader.PropertyToID("_RainbowIntensity");
    private static readonly int _RAINBOW_ENABLED_PROPERTY = Shader.PropertyToID("_RainbowEnabled");

    [Header("Rainbow Settings")]
    [Range(0f, 1f)]
    public float baseIntensity = 0.3f;
    
    [Range(1f, 3f)]
    public float movementBoost = 1.5f;
    
    [Range(0.01f, 0.5f)]
    public float movementSensitivity = 0.1f;
    
    [Range(0.1f, 2f)]
    public float boostFadeTime = 0.5f;
    
    [Range(0f, 0.95f)]
    public float smoothing = 0.8f;

    private Material _material;
    private Vector2 _smoothedMousePos = new(0.5f, 0.5f);
    private Vector2 _lastMousePos;
    private float _currentBoost = 1f;

    private void Start()
    {
        var rend = GetComponent<Renderer>();
        if (rend != null)
        {
            _material = rend.material;
        }
        
        if (_material == null)
        {
            enabled = false;
            return;
        }
        
        _lastMousePos = GetNormalizedMousePosition();
        _smoothedMousePos = _lastMousePos;
        
        _material.SetFloat(_RAINBOW_ENABLED_PROPERTY, 1f);
    }

    private void Update()
    {
        if (!_material) return;
        
        var currentMousePos = GetNormalizedMousePosition();
        
        var mouseSpeed = Vector2.Distance(currentMousePos, _lastMousePos) / Time.deltaTime;
        _lastMousePos = currentMousePos;
        
        var targetBoost = mouseSpeed > movementSensitivity ? movementBoost : 1f;
        _currentBoost = Mathf.Lerp(_currentBoost, targetBoost, Time.deltaTime / boostFadeTime);
        
        _smoothedMousePos = Vector2.Lerp(currentMousePos, _smoothedMousePos, smoothing);
        
        _material.SetVector(_MOUSE_POS_PROPERTY, new Vector4(_smoothedMousePos.x, _smoothedMousePos.y, 0, 0));
        _material.SetFloat(_RAINBOW_INTENSITY_PROPERTY, baseIntensity * _currentBoost);
    }

    private static Vector2 GetNormalizedMousePosition()
    {
        var mousePos = Input.mousePosition;
        return new Vector2(
            mousePos.x / Screen.width,
            mousePos.y / Screen.height
        );
    }

    private void OnDisable()
    {
        if (!_material) return;
        
        _material.SetFloat(_RAINBOW_ENABLED_PROPERTY,   0f);
        _material.SetFloat(_RAINBOW_INTENSITY_PROPERTY, 0f);
    }

    private void OnEnable()
    {
        if (!_material) return;
        
        _material.SetFloat(_RAINBOW_ENABLED_PROPERTY,   1f);
        _material.SetFloat(_RAINBOW_INTENSITY_PROPERTY, baseIntensity);
    }
}