using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class BulletComponent : ActorComponent
{
    public ActorComponent Owner;
    public Vector3 Velocity;
    public float MaxDuration = 5f;
    public float SpawnTime;

    public void Start()
    {
        SpawnTime = Time.time;
    }

    public void Update()
    {
        if (Time.time > SpawnTime + MaxDuration)
            Destroy(gameObject);

        transform.position += Time.deltaTime * Velocity;

        CollisionComponent collision = gameObject.GetComponentInSelfOrChildren<CollisionComponent>();
        
        Profiler.BeginSample("Get all intersections");
        IEnumerable<CollisionComponent> intersections = Game.CollisionSystem.GetIntersections(collision, CollisionType.Entity);
        Profiler.EndSample();
        
        foreach (var otherCollision in intersections)
        {
            Vector3 overlap = CollisionSystem.Overlap(collision, otherCollision);
            if (overlap != Vector3.zero)
                OnCollision(otherCollision);
        }
    }

    private void OnCollision(CollisionComponent otherCollision)
    {
        ActorComponent actor = otherCollision.GetOwner();

        if (actor != null && actor != Owner)
        {
            actor.ApplyDamage(1);
            GameObject.Destroy(gameObject);
        }
    }
}