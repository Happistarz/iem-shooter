using UnityEngine;
using UnityEngine.Serialization;

public class SinFactorComponent : MonoBehaviour
{
    [FormerlySerializedAs("Frequency")] public float frequency = 1f;
    [FormerlySerializedAs("Amplitude")] public float amplitude = 1f;

    private float _phaseOffset;

    private void Awake()
    {
        _phaseOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private float GetSinFactor(float time)
    {
        return Mathf.Sin(time * frequency + _phaseOffset) * amplitude;
    }

    public void Update()
    {
        var sinFactor = GetSinFactor(Time.time);

        var forward = transform.forward;
        var right   = Vector3.Cross(forward, Vector3.up).normalized;

        transform.position += right * (sinFactor * Time.deltaTime);
    }
}