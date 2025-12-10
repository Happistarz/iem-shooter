using UnityEngine;

public class BounceAnimationComponent : MonoBehaviour
{
    [Tooltip("Amplitude du bounce (hauteur)")]
    public float bounceAmplitude = 0.2f;

    [Tooltip("Durée du bounce en secondes")]
    public float bounceDuration = 0.15f;

    [Tooltip("Courbe d'animation du bounce")]
    public AnimationCurve bounceCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 _originalScale;
    private float   _bounceTimer;
    private bool    _isBouncing;

    private void Awake()
    {
        _originalScale = transform.localScale;
    }

    public void TriggerBounce()
    {
        _bounceTimer = 0f;
        _isBouncing  = true;
    }

    private void Update()
    {
        if (!_isBouncing) return;

        _bounceTimer += Time.deltaTime;
        var progress = Mathf.Clamp01(_bounceTimer / bounceDuration);

        var curveValue = bounceCurve.Evaluate(progress);

        var scaleFactor = 1f + bounceAmplitude * curveValue;
        transform.localScale = _originalScale * scaleFactor;

        if (!(progress >= 1f)) return;

        _isBouncing          = false;
        transform.localScale = _originalScale;
    }
}