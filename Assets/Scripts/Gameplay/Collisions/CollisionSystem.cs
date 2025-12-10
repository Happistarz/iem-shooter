using System.Collections.Generic;
using UnityEditor.AddressableAssets.Build.Layout;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Profiling;

public class CollisionSystem
{
    public SpatialGrid<List<CollisionComponent>> CollisionGrid;

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
        for (int i = 0; i < CollisionGrid.Data.Length; i++)
        {
            CollisionGrid.Data[i].Clear();
        }

        Profiler.EndSample();

        Profiler.BeginSample("Get all composition components");
        CollisionComponent[] allCollisions = GameObject.FindObjectsOfType<CollisionComponent>();
        Profiler.EndSample();

        Profiler.BeginSample("Register coliisions in the grid");
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
        float maxOtherCollisionRadius = 2;
        var collisions = new List<CollisionComponent>();

        var collisionPosition = new Vector2(collision.transform.position.x, collision.transform.position.z);
        Vector2 collisionMin = collisionPosition - Vector2.one * (maxOtherCollisionRadius + collision.Radius);
        Vector2 collisionMax = collisionPosition + Vector2.one * (maxOtherCollisionRadius + collision.Radius);
        var intersectingIndices = CollisionGrid.GetIndices(collisionMin, collisionMax);
        foreach (var index in intersectingIndices)
        {
            foreach (var otherCollision in CollisionGrid[index])
            {
                if (collision == otherCollision) continue;
                if (otherCollision.Type != type) continue;

                if (Intersection(collision, otherCollision))
                    collisions.Add(otherCollision);
            }
        }

        return collisions;
    }

    public bool Intersection(CollisionComponent c1, CollisionComponent c2)
    {
        Profiler.BeginSample("Test One Intersection");
        Vector3 separation = new Vector3(
            c2.transform.position.x - c1.transform.position.x, 0,
            c2.transform.position.z - c1.transform.position.z);
        float distance = separation.magnitude;
        Profiler.EndSample();

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