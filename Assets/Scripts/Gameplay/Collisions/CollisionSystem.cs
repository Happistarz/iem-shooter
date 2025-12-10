using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollisionSystem
{
    public static IEnumerable<CollisionComponent> GetIntersections(CollisionComponent collision, CollisionType type)
    {
        var allCollisions = Object.FindObjectsByType<CollisionComponent>(FindObjectsSortMode.None);

        return (allCollisions.Where(otherCollision => collision           != otherCollision)
                             .Where(otherCollision => otherCollision.Type == type)
                             .Where(otherCollision => Intersection(collision, otherCollision))).ToList();
    }

    public static bool Intersection(CollisionComponent c1, CollisionComponent c2)
    {
        var direction = new Vector3(
            c2.transform.position.x - c1.transform.position.x, 0,
            c2.transform.position.z - c1.transform.position.z);
        var distanceSquared = direction.sqrMagnitude;
        var radiusSum       = c1.Radius + c2.Radius;
        return distanceSquared < radiusSum * radiusSum;
    }

    public static Vector3 Overlap(CollisionComponent c1, CollisionComponent c2)
    {
        var direction = new Vector3(
            c2.transform.position.x - c1.transform.position.x, 0,
            c2.transform.position.z - c1.transform.position.z);

        var distance        = direction.magnitude;
        var radiusSum       = c1.Radius + c2.Radius;
        var overlapDistance = radiusSum - distance;

        if (overlapDistance <= 0f)
            return Vector3.zero;

        var overlapDirection = direction.normalized;
        return -overlapDirection * overlapDistance;
    }
}