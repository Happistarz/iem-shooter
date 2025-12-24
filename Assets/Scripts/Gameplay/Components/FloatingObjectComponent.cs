using UnityEngine;

public class FloatingObjectComponent : MonoBehaviour
{
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1.0f;

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;
    }

    private void Update()
    {
        var newY = _startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}