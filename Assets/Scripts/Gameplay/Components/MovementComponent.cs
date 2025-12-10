using UnityEngine;
using UnityEngine.Serialization;

public class MovementComponent : MonoBehaviour
{
    [FormerlySerializedAs("Speed")] public float speed;

    private Vector3 _direction;

    public void Update()
    {
        transform.position += Time.deltaTime * speed * _direction;
        if (_direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);

        var collision = gameObject.GetComponentInSelfOrChildren<CollisionComponent>();
        var intersections =
            CollisionSystem.GetIntersections(collision, CollisionType.Entity);
        foreach (var otherCollision in intersections)
        {
            var overlap = CollisionSystem.Overlap(collision, otherCollision);
            if (overlap != Vector3.zero)
                transform.position += 0.5f * overlap;
        }
    }

    public void SetMovementDirection(Vector3 moveDirection)
    {
        _direction = moveDirection;
    }
}