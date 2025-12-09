using System.Collections.Generic;
using UnityEngine;

public static class Game
{
    public static GameData Data;
        
    public static PlayerComponent Player;
    public static List<EnemyComponent> Enemies;
    
    public static CollisionSystem CollisionSystem;

    public static EnemyComponent GetClosestEnemy(Vector3 position)
    {
        EnemyComponent closestEnemy = null;
        float closestDistance = float.MaxValue;
        foreach (var enemy in Enemies)
        {
            var distance = Vector3.Distance(position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
}