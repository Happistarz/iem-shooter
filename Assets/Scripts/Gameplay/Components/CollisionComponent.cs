using UnityEngine;

public class CollisionComponent : MonoBehaviour
{
    public CollisionType Type;
    public float Radius;
    
    
    public ActorComponent GetOwner()
    {
        var actorComponent = GetComponent<ActorComponent>();
        if (actorComponent != null)
            return actorComponent;
        
        actorComponent = GetComponentInParent<ActorComponent>();
        return actorComponent != null ? actorComponent : null; // Retourner null au lieu de lever une exception
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        var points = new Vector3[32];
        for (var i = 0; i < 32; i++)
        {
            var angle = i * Mathf.PI * 2 / 32;
            points[i] = transform.position + new Vector3(Mathf.Cos(angle) * Radius, 0, Mathf.Sin(angle) * Radius);
        }
        
        Gizmos.DrawLineStrip(points,true);
    }
}