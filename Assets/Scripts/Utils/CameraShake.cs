using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Vector3 _initialPosition;
    private Quaternion _initialRotation;
    private bool _isShaking;

    public void Shake(float duration = 0.2f, float magnitude = 0.3f)
    {
        if (_isShaking)
            StopCoroutine(ShakeCoroutine(duration, magnitude));
        
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        _isShaking = true;
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
        
        var elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            
            var dampingFactor = Mathf.Clamp01(1f - elapsedTime / duration);
            
            var randomOffset = Random.insideUnitSphere * (magnitude * dampingFactor);
            transform.localPosition = _initialPosition + randomOffset;

            yield return null;
        }

        transform.localPosition = _initialPosition;
        transform.localRotation = _initialRotation;
        _isShaking = false;
    }
}

