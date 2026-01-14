using UnityEngine;
using UnityEngine.Serialization;

public class ActorComponent : MonoBehaviour
{
    [FormerlySerializedAs("Health")] public int health = 1;
    public int maxHealth;

    [FormerlySerializedAs("CanTakeDamage")]
    public bool canTakeDamage = true;

    protected virtual void Awake()
    {
        maxHealth = health;
    }

    public virtual void ApplyDamage(int damage)
    {
        if (!canTakeDamage) return;

        health -= damage;
        if (health > 0) return;
        
        OnDeath();
    }

    protected virtual void OnDeath()
    {
        Destroy(gameObject);
    }
}