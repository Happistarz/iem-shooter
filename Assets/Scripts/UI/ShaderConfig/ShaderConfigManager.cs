using UnityEngine;
using System.Collections.Generic;

public class ShaderConfigManager : MonoBehaviour
{
    public static ShaderConfigManager Instance { get; private set; }

    private static readonly int _TEXTURE_INFLUENCE = Shader.PropertyToID("_TextureInfluence");
    private static readonly int _BRIGHTNESS_TO_ALPHA = Shader.PropertyToID("_BrightnessToAlpha");
    private static readonly int _SCAN_LINE_COUNT = Shader.PropertyToID("_ScanLineCount");
    private static readonly int _FLICKER_INTENSITY = Shader.PropertyToID("_FlickerIntensity");
    
    private readonly List<Material> _registeredMaterials = new();

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterMaterial(Material material)
    {
        if (!material || _registeredMaterials.Contains(material)) return;
        
        _registeredMaterials.Add(material);
        ApplyConfigToMaterial(material);
    }

    public void UnregisterMaterial(Material material)
    {
        _registeredMaterials.Remove(material);
    }
    
    public void ApplyConfigToAll()
    {
        _registeredMaterials.RemoveAll(m => !m);

        foreach (var material in _registeredMaterials)
        {
            ApplyConfigToMaterial(material);
        }
    }
    
    private void ApplyConfigToMaterial(Material material)
    {
        if (!material) return;

        // Rainbow
        material.SetFloat(_TEXTURE_INFLUENCE, ShaderConfig.TextureInfluence);
        material.SetFloat(_BRIGHTNESS_TO_ALPHA, ShaderConfig.BrightnessToAlpha);
        material.SetFloat(_SCAN_LINE_COUNT, ShaderConfig.ScanLineCount);
        material.SetFloat(_FLICKER_INTENSITY, ShaderConfig.FlickerIntensity);
    }
    
    public void ResetConfig()
    {
        ShaderConfig.ResetToDefaults();
        ApplyConfigToAll();
    }
}
