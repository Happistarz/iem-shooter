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
    }

    protected override void OnDeath()
    {
        Game.GetEnemyPool(enemyData).Release(this);
    }

    private void OnDestroy()
    {
        Game.Enemies.Remove(this);
    }
}