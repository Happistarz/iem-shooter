using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Game
{
    public static GameData Data;

    public static PlayerComponent      Player;
    public static List<EnemyComponent> Enemies;

    public static CollisionSystem CollisionSystem;

    public static readonly Dictionary<EnemyData, PrefabPool<EnemyComponent>> ENEMY_PREFAB_POOLS = new();
    public static          PrefabPool<BulletComponent>                       BulletPrefabPool;

    public static EnemyComponent GetClosestEnemy(Vector3 position)
    {
        EnemyComponent closestEnemy    = null;
        var            closestDistance = float.MaxValue;
        foreach (var enemy in Enemies)
        {
            var distance = Vector3.Distance(position, enemy.transform.position);
            if (!(distance < closestDistance)) continue;

            closestDistance = distance;
            closestEnemy    = enemy;
        }

        return closestEnemy;
    }

    public static PrefabPool<EnemyComponent> GetEnemyPool(EnemyData enemy)
    {
        var enemyData = Data.Enemies.FirstOrDefault(e => e == enemy);
        return enemyData == null ? null : ENEMY_PREFAB_POOLS[enemyData];
    }
}