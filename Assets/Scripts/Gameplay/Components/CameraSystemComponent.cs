using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraSystemComponent : MonoBehaviour
{
    public CinemachineImpulseSource impulseSource;

    public void ShakeCamera(float force = 1.0f, float duration = 0.5f)
    {
        if (!impulseSource)
        {
            Debug.LogWarning("Impulse Source not assigned on CameraSystemComponent.");
            return;
        }

        StartCoroutine(ShakeCameraCoroutine(force, duration));
    }

    private IEnumerator ShakeCameraCoroutine(float force, float duration)
    {
        var elapsed = 0f;

        const float INTERVAL = 0.1f;
        while (elapsed < duration)
        {
            impulseSource.GenerateImpulse(force);
            yield return new WaitForSeconds(INTERVAL);
            elapsed += INTERVAL;
        }
    }
}