using UnityEngine;

public class ColorGradientComponent : MonoBehaviour
{
    public float TimeOffset;
    public Gradient Gradient;
    public float Duration;

    public void Update()
    {
        var renderer = GetComponent<MeshRenderer>();
        if (renderer == null) return;
        
        var gradientTime = ((Time.time + TimeOffset)) % Duration / (Duration);

        var material = renderer.material;
        material.color = Gradient.Evaluate(gradientTime);
    }
}