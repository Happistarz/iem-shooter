using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorGradientComponent : MonoBehaviour
{
    public float TimeOffset;
    public Gradient Gradient;
    public float Duration;

    public void Update()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            var gradientTime = ((Time.time + TimeOffset)) % Duration / (Duration);

            var material = renderer.material;
            material.color = Gradient.Evaluate(gradientTime);
        }
    }
}