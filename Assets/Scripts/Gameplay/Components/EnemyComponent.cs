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
        PlayerComponent player = GameObject.FindObjectOfType<PlayerComponent>();
        Vector3 moveDirection = Vector3.zero;
        if (player != null)
            moveDirection = (player.transform.position - transform.position).normalized;
            
        m_movementComponent.SetMovementDirection(moveDirection);
    }

    private void OnDestroy()
    {
        Game.Enemies.Remove(this);
    }
}