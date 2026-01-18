using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Weapons;

public class PlayerComponent : ActorComponent
{
    private readonly List<WeaponBehaviour> _activeWeapons = new();

    private MovementComponent _movementComponent;
    private BounceAnimationComponent _bounceAnimation;
    private CollisionComponent _collisionComponent;
    
    public AudioSource ShootAudioSource;
    public AudioSource HitAudioSource;

    [Header("Player Health Settings")]
    public float invulnerabilityDuration = 2.0f;
    public float blinkSpeed = 0.1f;
    public Renderer playerRenderer;

    private bool _isInvulnerable;

    public void Start()
    {
        _movementComponent = GetComponent<MovementComponent>();
        _bounceAnimation = GetComponent<BounceAnimationComponent>();
        _collisionComponent = GetComponent<CollisionComponent>();

        if (Game.Data.StartingWeapon)
            AddWeapon(Game.Data.StartingWeapon);

        if (Game.UI)
        {
            Game.UI.ShowHealth(health, maxHealth);
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
        Game.UI.ShowHealth(health, maxHealth);
    }

    public override void ApplyDamage(int damage)
    {
        if (_isInvulnerable) return;

        base.ApplyDamage(damage);
        
        if (Game.UI)
        {
            Game.UI.ShowHealth(health, maxHealth);
            Game.UI.ShowDamageOverlay();
        }

        if (health > 0)
        {
            StartCoroutine(InvulnerabilityCoroutine());
        }
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        _isInvulnerable = true;
        canTakeDamage = false;
        
        HitAudioSource.Play();

        if (playerRenderer)
        {
            var originalColor = playerRenderer.material.color;
            Tween blinkTween = playerRenderer.material
                                             .DOColor(Color.red, blinkSpeed)
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
        canTakeDamage = true;
    }

    private void OnWeaponShoot()
    {
        ShootAudioSource.Play();
        
        if (_bounceAnimation)
            _bounceAnimation.TriggerBounce();
    }

    public void Update()
    {
        if (Game.IsGamePaused) return;
        
        CheckEnemyCollisions();

        var inputHorizontal = 0.0f;
        if (Input.GetKey(KeyCode.RightArrow)) inputHorizontal++;
        if (Input.GetKey(KeyCode.LeftArrow)) inputHorizontal--;

        var inputVertical = 0.0f;
        if (Input.GetKey(KeyCode.UpArrow)) inputVertical++;
        if (Input.GetKey(KeyCode.DownArrow)) inputVertical--;

        var moveDirection = new Vector3(inputHorizontal, 0, inputVertical).normalized;
        _movementComponent.SetMovementDirection(moveDirection);

        var shootDirection = GetShootDirection();
        foreach (var weapon in _activeWeapons)
        {            
            weapon.UpdateWeapon(transform.position, shootDirection);
        }
    }

    private Vector3 GetShootDirection()
    {
        var      shootDirection = Vector3.zero;
        Vector3? targetPosition = null;

        var closestEnemy = Game.GetClosestEnemy(transform.position);
        var distToEnemy = closestEnemy ? Vector3.Distance(transform.position, closestEnemy.transform.position) : float.MaxValue;

        // Check for boss fight priority target
        if (GameLoop.Instance && GameLoop.Instance.bossFight && GameLoop.Instance.bossFight.gameObject.activeInHierarchy)
        {
            var distToBoss = Vector3.Distance(transform.position, GameLoop.Instance.bossFight.transform.position);
            if (distToBoss < distToEnemy)
            {
                targetPosition = GameLoop.Instance.bossFight.transform.position;
            }
        }
    
        // If no boss fight target, aim for closest enemy
        if (targetPosition == null && closestEnemy)
        {
            targetPosition = closestEnemy.transform.position;
        }

        // If we have a target, calculate direction
        if (targetPosition.HasValue)
        {
            shootDirection = (targetPosition.Value - transform.position).normalized;
        }
        
        return shootDirection;
    }

    private void CheckEnemyCollisions()
    {
        if (_isInvulnerable || health <= 0) return;
        if (!_collisionComponent) return;

        var collisions = Game.CollisionSystem.GetIntersections(_collisionComponent, CollisionType.Entity);
        foreach (var collision in collisions)
        {
            var enemyActor = collision.GetOwner();
            if (enemyActor is EnemyComponent enemy)
            {
                ApplyDamage(enemy.enemyData.Damage);
                break;
            }

            if (enemyActor is not BossFightComponent) continue;
            
            ApplyDamage(1);
            break;
        }
    }
    
    public void AddWeapon(WeaponData weaponData)
    {
        var behaviour = weaponData.AttachWeapon(gameObject, this);
        _activeWeapons.Add(behaviour);
        
        behaviour.OnShoot += OnWeaponShoot;
    }

    public void ReplaceWeapon(WeaponData oldWeapon, WeaponData newWeapon)
    {
        var weaponToRemove = _activeWeapons.FirstOrDefault(w => w.data == oldWeapon);
        if (weaponToRemove)
        {
            Destroy(weaponToRemove.gameObject);
            _activeWeapons.Remove(weaponToRemove);
        }
        AddWeapon(newWeapon);
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        GameLoop.Instance.OnPlayerDefeated();
    }
}