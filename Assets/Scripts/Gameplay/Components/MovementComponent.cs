using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class MovementComponent : MonoBehaviour
{
    public float Speed;
    private Vector3 m_direction;

    public void Update()
    {
        Profiler.BeginSample("Update Transform");
        transform.position += Time.deltaTime * Speed * m_direction;
        if (m_direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(m_direction, Vector3.up);
        Profiler.EndSample();
        
        Profiler.BeginSample("Get our own Collision Component");
        CollisionComponent collision = gameObject.GetComponentInSelfOrChildren<CollisionComponent>();
        Profiler.EndSample();
        
        Profiler.BeginSample("Get all intersections");
        IEnumerable<CollisionComponent> intersections = Game.CollisionSystem.GetIntersections(collision, CollisionType.Entity);
        Profiler.EndSample();
        
        Profiler.BeginSample("React to collisions");
        foreach (var otherCollision in intersections)
        {
            Vector3 overlap = CollisionSystem.Overlap(collision, otherCollision);
            if (overlap != Vector3.zero)
                transform.position += 0.5f * overlap;
        }
        Profiler.EndSample();
    }

    public void SetMovementDirection(Vector3 moveDirection)
    {
        m_direction = moveDirection;
    }
}