using UnityEngine;

public class EnemyComponent : ActorComponent
{
    private MovementComponent _movementComponent;
    public  EnemyData         enemyData;

    public void Start()
    {
        Game.Enemies.Add(this);
        _movementComponent = GetComponent<MovementComponent>();
    }

    public void Update()
    {
        var player        = FindFirstObjectByType<PlayerComponent>();
        var moveDirection = Vector3.zero;
        if (player != null)
            moveDirection = (player.transform.position - transform.position).normalized;

        _movementComponent.SetMovementDirection(moveDirection);
        _movementComponent.SetSpeed(enemyData.MoveSpeed);
    }

    protected override void OnDeath()
    {
        Game.AudioManager.PlaySoundAtPosition(enemyData.DeathSound, transform.position);
        Game.GetEnemyPool(enemyData).Release(this);
    }

    private void OnDestroy()
    {
        Game.Enemies.Remove(this);
    }
}