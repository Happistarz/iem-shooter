using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

public class EnemySpawnAnimationComponent : MonoBehaviour
{
    public MovementComponent movementComponent;

    public float animationDuration = 1.2f;
    public float heightOffset      = 90f;

    public TweenCallback OnSpawnComplete = null;

    private void OnEnable()
    {
        if (movementComponent)
        {
            movementComponent.canMove = false;
        }

        transform.DOMoveY(0.0f, animationDuration).From(heightOffset).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            OnSpawnComplete?.Invoke();
            if (OnSpawnComplete == null)
            {
                DefaultOnSpawnComplete();
            }
        });
    }

    private void DefaultOnSpawnComplete()
    {
        if (movementComponent)
        {
            movementComponent.canMove = true;
        }
    }
}