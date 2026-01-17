using UnityEngine;

public class BulletComponent : ActorComponent
{
    public ActorComponent Owner;
    public Vector3        Velocity;
    public float          MaxDuration = 5f;
    public float          SpawnTime;
    public int            Damage = 1;

    private Vector3 _localScale;

    public BulletComponent prefab;

    protected override void Awake()
    {
        _localScale = transform.localScale;
    }

    private void OnEnable()
    {
        Reset();
    }

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
        var intersections = Game.CollisionSystem.GetIntersections(collision, CollisionType.Entity);
        foreach (var otherCollision in intersections)
        {
            var overlap = CollisionSystem.Overlap(collision, otherCollision);
            if (overlap != Vector3.zero)
                OnCollision(otherCollision);
        }
    }

    private void OnCollision(CollisionComponent otherCollision)
    {
        var actor = otherCollision?.GetOwner();

        if (actor is null || actor == Owner) return;
        if (actor is BulletComponent) return;

        var isOwnerEnemy  = Owner is EnemyComponent or BossFightComponent;
        var isTargetEnemy = actor is EnemyComponent or BossFightComponent;

        if (isOwnerEnemy && isTargetEnemy) return;

        actor.ApplyDamage(Damage);
        OnDeath();
    }

    public void Reset()
    {
        Owner                = null;
        Velocity             = Vector3.zero;
        SpawnTime            = Time.time;
        transform.localScale = _localScale;
    }

    protected override void OnDeath()
    {
        if (!prefab)
        {
            Destroy(gameObject);
            return;
        }

        var pool = Game.GetBulletPool(prefab);
        pool.Release(this);
    }
}