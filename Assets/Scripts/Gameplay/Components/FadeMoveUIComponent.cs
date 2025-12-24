using DG.Tweening;
using UnityEngine;

public class FadeMoveUIComponent : MonoBehaviour
{
    public Ease  moveEase     = Ease.OutCubic;
    public float moveDuration = 0.5f;
    public float offsetY      = 50f;
    public float delay;
    public float delayFadeOut;
    public bool  moveDown;

    private RectTransform _rectTransform;

    private float _startY;
    private float _targetY;

    private void Awake()
    {
        if (!_rectTransform)
            _rectTransform = GetComponent<RectTransform>();

        if (moveDown)
            _startY  = _rectTransform.anchoredPosition.y + _rectTransform.rect.height + offsetY;
        else
            _startY  = _rectTransform.anchoredPosition.y - _rectTransform.rect.height - offsetY;

        _targetY = _rectTransform.anchoredPosition.y;
    }

    public void FadeIn()
    {
        if (!_rectTransform) return;

        _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, _startY);
        _rectTransform.DOKill();
        _rectTransform.DOAnchorPosY(_targetY, moveDuration).SetEase(moveEase).SetDelay(delay);
    }

    public void FadeOut()
    {
        if (!_rectTransform) return;

        _rectTransform.DOKill();
        _rectTransform.DOAnchorPosY(_startY, moveDuration).SetEase(moveEase).SetDelay(delayFadeOut);
    }
}