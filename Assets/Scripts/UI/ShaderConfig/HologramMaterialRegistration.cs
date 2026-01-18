using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HologramMaterialRegistration : MonoBehaviour
{
    private Material _material;

    private void Start()
    {
        var rend = GetComponent<Renderer>();
        if (!rend) return;
        
        _material = rend.material;
            
        if (ShaderConfigManager.Instance != null)
        {
            ShaderConfigManager.Instance.RegisterMaterial(_material);
        }
    }

    private void OnDestroy()
    {
        if (_material && ShaderConfigManager.Instance != null)
        {
            ShaderConfigManager.Instance.UnregisterMaterial(_material);
        }
    }
}

