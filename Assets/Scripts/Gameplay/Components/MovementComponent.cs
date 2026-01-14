using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    public bool canMove = true;

    public float speed;

    private Vector3 _direction;
    private CollisionComponent _collisionComponent;
    
    public void Start()
    {
        _collisionComponent = gameObject.GetComponentInSelfOrChildren<CollisionComponent>();
    }

    public void Update()
    {
        if (Game.IsGamePaused) return;
        
        if (!canMove)
            return;

        transform.position += Time.deltaTime * speed * _direction;
        if (_direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);

        var intersections = Game.CollisionSystem.GetIntersections(_collisionComponent, CollisionType.Entity);
        
        foreach (var otherCollision in intersections)
        {
            var overlap = CollisionSystem.Overlap(_collisionComponent, otherCollision);
            if (overlap == Vector3.zero) continue;

            var pushFactor = otherCollision.IsStatic ? 1.01f : 0.5f;
            transform.position += pushFactor * overlap;
        }
    }

    public void SetMovementDirection(Vector3 moveDirection)
    {
        _direction = moveDirection;
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}