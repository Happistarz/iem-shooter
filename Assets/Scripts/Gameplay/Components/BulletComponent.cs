using UnityEngine;

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
        {
            OnDeath();
            return;
        }
        
        transform.position += Time.deltaTime * Velocity;

        var collision     = gameObject.GetComponentInSelfOrChildren<CollisionComponent>();
        var intersections = CollisionSystem.GetIntersections(collision, CollisionType.Entity);
        foreach (var otherCollision in intersections)
        {
            var overlap = CollisionSystem.Overlap(collision, otherCollision);
            if (overlap != Vector3.zero)
                OnCollision(otherCollision);
        }
    }

    private void OnCollision(CollisionComponent otherCollision)
    {
        var actor = otherCollision.GetOwner();

        if (actor == null || actor == Owner) return;
        actor.ApplyDamage(1);
        OnDeath();
    }
    
    protected override void OnDeath()
    {
        Game.BulletPrefabPool.Release(this);
    }
}