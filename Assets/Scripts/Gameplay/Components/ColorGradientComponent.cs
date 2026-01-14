using UnityEngine;

public class ColorGradientComponent : MonoBehaviour
{
    public float TimeOffset;
    public Gradient Gradient;
    public float Duration;
    
    private Renderer _meshRenderer;
    
    public void Update()
    {
        _meshRenderer ??= GetComponent<MeshRenderer>();
        
        var gradientTime = (Time.time + TimeOffset) % Duration / Duration;

        var material = _meshRenderer.material;
        material.color = Gradient.Evaluate(gradientTime);
    }
}