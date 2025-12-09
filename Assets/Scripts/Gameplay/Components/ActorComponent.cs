using UnityEngine;

public class ActorComponent : MonoBehaviour
{
    public int Health = 1;
    public bool CanTakeDamage;
    
    public void ApplyDamage(int damage)
    {
        if (!CanTakeDamage) return;
        
        Health -= damage;
        if (Health <= 0)
        {
            Destroy(gameObject);
        }
    }
}