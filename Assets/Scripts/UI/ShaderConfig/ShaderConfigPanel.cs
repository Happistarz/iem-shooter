using UnityEngine;

public class ShaderConfigPanel : MonoBehaviour
{
    public static ShaderConfigPanel Instance { get; private set; }
    
    [Header("Panel")]
    public GameObject panelRoot;
    public KeyCode toggleKey = KeyCode.Tab;

    [Header("Preview 3D")]
    public GameObject previewObject;

    private bool _isInMenu;

    public bool IsOpen => panelRoot && panelRoot.activeSelf;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (panelRoot)
            panelRoot.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey) && !_isInMenu)
        {
            TogglePanel();
        }

        // Rotation du cube preview
        if (previewObject && IsOpen)
        {
            previewObject.transform.Rotate(Vector3.up, 45f * Time.deltaTime);
        }
    }

    public void SetTextureInfluence(float value)
    {
        ShaderConfig.TextureInfluence = value;
        ApplyChanges();
    }

    public void SetBrightnessToAlpha(float value)
    {
        ShaderConfig.BrightnessToAlpha = value;
        ApplyChanges();
    }

    public void SetScanLineCount(float value)
    {
        ShaderConfig.ScanLineCount = value;
        ApplyChanges();
    }

    public void SetFlickerIntensity(float value)
    {
        ShaderConfig.FlickerIntensity = value;
        ApplyChanges();
    }

    private static void ApplyChanges()
    {
        ShaderConfigManager.Instance?.ApplyConfigToAll();
    }
    
    public void ResetToDefaults()
    {
        ShaderConfig.ResetToDefaults();
        
        var sliders = Instance.GetComponentsInChildren<UnityEngine.UI.Slider>();
        foreach (var slider in sliders)
        {
            slider.value = slider.name switch
            {
                "TextureInfluenceSlider"  => ShaderConfig.TextureInfluence,
                "BrightnessToAlphaSlider" => ShaderConfig.BrightnessToAlpha,
                "ScanLineCountSlider"     => ShaderConfig.ScanLineCount,
                "FlickerIntensitySlider"  => ShaderConfig.FlickerIntensity,
                _                         => slider.value
            };
        }
    }
    
    private void TogglePanel()
    {
        if (IsOpen)
            ClosePanel();
        else
            OpenPanel();
    }

    public void OpenPanel()
    {
        if (!panelRoot) return;
        panelRoot.SetActive(true);
    }

    public void ClosePanel()
    {
        if (!panelRoot) return;
        panelRoot.SetActive(false);
    }
}
