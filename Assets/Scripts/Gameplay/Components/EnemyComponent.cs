using UnityEngine;

public class EnemyComponent : ActorComponent
{
    private MovementComponent m_movementComponent;

    public void Start()
    {
        Game.Enemies.Add(this);
        m_movementComponent = GetComponent<MovementComponent>();
    }

    public void Update()
    {
        Vector3 moveDirection = Vector3.zero;
        moveDirection = (Game.Player.transform.position - transform.position).normalized;

        m_movementComponent.SetMovementDirection(moveDirection);
    }

    private void OnDestroy()
    {
        Game.Enemies.Remove(this);
    }
}