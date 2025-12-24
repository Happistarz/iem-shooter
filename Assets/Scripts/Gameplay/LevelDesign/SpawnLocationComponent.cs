using UnityEngine;

public class SpawnLocationComponent : MonoBehaviour
{
    public float radius = 3f;
    
    public Vector3 GetSpawnPosition()
    {
        var randomPoint = Random.insideUnitCircle * radius;
        return transform.position + new Vector3(randomPoint.x, 0f, randomPoint.y);
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 1.5f);
    }
}