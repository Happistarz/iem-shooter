using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

public class CollisionSystem
{
    public readonly SpatialGrid<List<CollisionComponent>> CollisionGrid;

    public CollisionSystem(float extent, int cellCount)
    {
        CollisionGrid = new SpatialGrid<List<CollisionComponent>>(
            -Vector2.one * 0.5f * extent, Vector2.one * 0.5f * extent,
            cellCount, cellCount);

        foreach (var index in CollisionGrid.Indices)
        {
            CollisionGrid[index] = new List<CollisionComponent>();
        }
    }

    public void UpdateGrid()
    {
        Profiler.BeginSample("Rebuild the collision grid");

        Profiler.BeginSample("Clear existing data");
        foreach (var data in CollisionGrid.Data)
        {
            data.Clear();
        }

        Profiler.EndSample();

        Profiler.BeginSample("Get all composition components");
        var allCollisions = Object.FindObjectsByType<CollisionComponent>(FindObjectsSortMode.None);
        Profiler.EndSample();

        Profiler.BeginSample("Register collisions in the grid");
        foreach (var collisionCmp in allCollisions)
        {
            var collisionPosition = new Vector2(collisionCmp.transform.position.x, collisionCmp.transform.position.z);
            if (CollisionGrid.Contains(collisionPosition))
                CollisionGrid[collisionPosition].Add(collisionCmp);
        }

        Profiler.EndSample();

        Profiler.EndSample();
    }

    public IEnumerable<CollisionComponent> GetIntersections(CollisionComponent collision, CollisionType type)
    {
        const float MAX_OTHER_COLLISION_RADIUS = 2;

        var collisions = new List<CollisionComponent>();

        if (collision is null) return collisions;

        var collisionPosition   = new Vector2(collision.transform.position.x, collision.transform.position.z);
        var collisionMin        = collisionPosition - Vector2.one * (MAX_OTHER_COLLISION_RADIUS + collision.Radius);
        var collisionMax        = collisionPosition + Vector2.one * (MAX_OTHER_COLLISION_RADIUS + collision.Radius);
        var intersectingIndices = CollisionGrid.GetIndices(collisionMin, collisionMax);
        collisions.AddRange(
            from index in intersectingIndices
            from otherCollision in CollisionGrid[index]
            where otherCollision != null && collision != otherCollision
            where otherCollision.Type == type
            where Intersection(collision, otherCollision)
            select otherCollision);

        return collisions;
    }

    public static bool Intersection(CollisionComponent c1, CollisionComponent c2)
    {
        if (c1 is null || c2 is null) return false;

        Profiler.BeginSample("Test One Intersection");
        var separation = new Vector3(
            c2.transform.position.x - c1.transform.position.x, 0,
            c2.transform.position.z - c1.transform.position.z);
        var distance = separation.magnitude;
        Profiler.EndSample();

        return distance < c1.Radius + c2.Radius;
    }

    public static Vector3 Overlap(CollisionComponent c1, CollisionComponent c2)
    {
        if (c1 is null || c2 is null) return Vector3.zero;

        var direction = new Vector3(
            c2.transform.position.x - c1.transform.position.x, 0,
            c2.transform.position.z - c1.transform.position.z);

        var distance = direction.magnitude;
        if (distance < c1.Radius + c2.Radius)
            return -direction.normalized * (c1.Radius + c2.Radius - distance);

        return Vector3.zero;
    }
}