using System.Collections.Generic;
using UnityEngine;

public class CollisionSystem
{
    public IEnumerable<CollisionComponent> GetIntersections(CollisionComponent collision, CollisionType type)
    {
        CollisionComponent[] allCollisions = GameObject.FindObjectsOfType<CollisionComponent>();
        var collisions = new List<CollisionComponent>();

        foreach (var otherCollision in allCollisions)
        {
            if (collision == otherCollision) continue;
            if (otherCollision.Type != type) continue;

            if (Intersection(collision, otherCollision))
                collisions.Add(otherCollision);
        }

        return collisions;
    }

    public bool Intersection(CollisionComponent c1, CollisionComponent c2)
    {
        Vector3 direction = new Vector3(
            c2.transform.position.x - c1.transform.position.x, 0, 
            c2.transform.position.z - c1.transform.position.z);
        float distance = direction.magnitude;
        return distance < c1.Radius + c2.Radius;
    }

    public Vector3 Overlap(CollisionComponent c1, CollisionComponent c2)
    {
        Vector3 direction = new Vector3(
            c2.transform.position.x - c1.transform.position.x, 0, 
            c2.transform.position.z - c1.transform.position.z);
        float distance = direction.magnitude;
        if (distance < c1.Radius + c2.Radius)
            return -direction.normalized * (c1.Radius + c2.Radius - distance);
        
        return Vector3.zero;
    }
}