using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlayerHealthComponent : ActorComponent
{
    [Header("Player Settings")]
    public float invulnerabilityDuration = 2.0f;

    public float blinkSpeed = 0.1f;

    [Header("References")]
    public Renderer playerRenderer;

    private bool               _isInvulnerable;
    private CollisionComponent _playerCollision;
    
    private float _maxHealth;

    private void Start()
    {
        canTakeDamage    = true;
        _playerCollision = GetComponent<CollisionComponent>();

        if (health <= 0)
        {
            health = 20;
        }
        
        _maxHealth = health;
        
        if (Game.UI != null)
        {
            Game.UI.ShowHealth(health, _maxHealth);
        }
        else
        {
            StartCoroutine(WaitForUI());
        }
    }

    private IEnumerator WaitForUI()
    {
        yield return new WaitForEndOfFrame();
        while (!Game.UI) yield return null;
        Game.UI.ShowHealth(health, _maxHealth);
    }

    private void Update()
    {
        if (health <= 0 || _isInvulnerable) return;

        CheckEnemyCollisions();
    }

    private void CheckEnemyCollisions()
    {
        var collisions = Game.CollisionSystem.GetIntersections(_playerCollision, CollisionType.Entity);
        foreach (var collision in collisions)
        {
            var enemyActor = collision.GetOwner();

            if (enemyActor is not EnemyComponent enemy) continue;

            TakeHit(enemy.enemyData.Damage);
            break;
        }
    }

    public void TakeHit(int damage)
    {
        if (_isInvulnerable) return;

        ApplyDamage(damage);
        
        if (Game.UI)
        {
            Game.UI.ShowDamageOverlay();
            Game.UI.ShowHealth(health, _maxHealth);
        }

        StartCoroutine(InvulnerabilityCoroutine());
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        _isInvulnerable = true;
        canTakeDamage   = false;

        if (playerRenderer)
        {
            var originalColor = playerRenderer.material.color;
            Tween blinkTween = playerRenderer.material
                                             .DOColor(
                                                 Color.red,
                                                 blinkSpeed)
                                             .SetLoops(-1, LoopType.Yoyo);

            yield return new WaitForSeconds(invulnerabilityDuration);

            blinkTween.Kill();
            playerRenderer.material.color = originalColor;
        }
        else
        {
            yield return new WaitForSeconds(invulnerabilityDuration);
        }

        _isInvulnerable = false;
        canTakeDamage   = true;
    }
}