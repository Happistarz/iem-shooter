using DG.Tweening;
using UnityEngine;

public class TerrainCorruptionComponent : MonoBehaviour
{
    public static readonly int CORRUPTION_AMOUNT = Shader.PropertyToID("_CorruptionAmount");
    
    public float duration = 1.0f;
    
    private Renderer _renderer;
    
    public void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }
    
    public void CorruptTerrain()
    {
        var material = _renderer.material;
        material.DOFloat(1.0f, CORRUPTION_AMOUNT, duration).SetEase(Ease.InOutSine);
    }
    
    public void RestoreTerrain()
    {
        var material = _renderer.material;
        material.DOFloat(0.0f, CORRUPTION_AMOUNT, duration).SetEase(Ease.InOutSine);
    }
}