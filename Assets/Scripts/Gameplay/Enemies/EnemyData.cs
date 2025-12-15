using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy Data")]
public class EnemyData : ScriptableObject
{
    public enum ThreatLevel
    {
        Simple, Medium, Advanced, Boss
    }
    
    public string Name;
    public EnemyComponent Prefab;
    public ThreatLevel Threat;
    
    [Range(1, 10)]
    public int Rarity = 5;

    public int Health = 1;
    
    public float MoveSpeed = 2f;
}