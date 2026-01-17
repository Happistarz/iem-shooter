using UnityEngine;
using UnityEngine.Serialization;

public class SinFactorComponent : MonoBehaviour
{
    [FormerlySerializedAs("Frequency")] public float frequency = 1f;
    [FormerlySerializedAs("Amplitude")] public float amplitude = 1f;
    
    public MovementComponent movement;
    public Vector3 direction = Vector2.right;

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
        if (movement && movement.canMove)
            return;
        
        var sinFactor = GetSinFactor(Time.time);

        transform.position += direction.normalized * (sinFactor * Time.deltaTime);
    }
}