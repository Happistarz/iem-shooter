using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    public float Speed;
    private Vector3 m_direction;

    public void Update()
    {
        transform.position += Time.deltaTime * Speed * m_direction;
        if (m_direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(m_direction, Vector3.up);
        
        CollisionComponent collision = gameObject.GetComponentInSelfOrChildren<CollisionComponent>();
        IEnumerable<CollisionComponent> intersections = Game.CollisionSystem.GetIntersections(collision, CollisionType.Entity);
        foreach (var otherCollision in intersections)
        {
            Vector3 overlap = Game.CollisionSystem.Overlap(collision, otherCollision);
            if (overlap != Vector3.zero)
                transform.position += 0.5f * overlap;
        }
    }

    public void SetMovementDirection(Vector3 moveDirection)
    {
        m_direction = moveDirection;
    }
}