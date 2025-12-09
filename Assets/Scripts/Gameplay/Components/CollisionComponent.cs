using System;
using UnityEngine;

public class CollisionComponent : MonoBehaviour
{
    public CollisionType Type;
    public float Radius;
    
    
    public ActorComponent GetOwner()
    {
        ActorComponent actorComponent = GetComponent<ActorComponent>();
        if (actorComponent != null)
            return actorComponent;
        
        actorComponent = GetComponentInParent<ActorComponent>();
        if (actorComponent != null)
            return actorComponent;

        throw new Exception("Cannot find collision owner");
    }
    
    public Vector3 GetOverlapOffset(CollisionComponent other)
    {
        Vector3 direction = new Vector3(other.transform.position.x - transform.position.x, 0, other.transform.position.z - transform.position.z);
        float distance = direction.magnitude;
        if (distance < Radius + other.Radius)
        {
            return -direction.normalized * (Radius + other.Radius - distance);
        }

        return Vector3.zero;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        Vector3[] points = new Vector3[32];
        for (int i = 0; i < 32; i++)
        {
            float angle = i * Mathf.PI * 2 / 32;
            points[i] = transform.position + new Vector3(Mathf.Cos(angle) * Radius, 0, Mathf.Sin(angle) * Radius);
        }
        
        Gizmos.DrawLineStrip(points,true);
    }
}