using DG.Tweening;
using UnityEngine;

public class EnemySpawnAnimationComponent : MonoBehaviour
{
    public MovementComponent movementComponent;
    public Renderer          enemyRenderer;

    public float animationDuration = 1.0f;
    public float heightOffset      = 5.0f;

    private void OnEnable()
    {
        if (movementComponent != null)
        {
            movementComponent.canMove = false;
        }

        transform.DOMoveY(0.0f, animationDuration).From(heightOffset).SetEase(Ease.InOutCubic).OnComplete(() =>
        {
            if (movementComponent != null)
            {
                movementComponent.canMove = true;
            }
        });
    }
}