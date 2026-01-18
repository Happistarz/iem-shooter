using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Game
{
    public static GameData Data;
    public static bool IsGamePaused;

    public static UIManager UI;

    public static PlayerComponent      Player;
    public static List<EnemyComponent> Enemies;
    
    public static TractorBeamsController TractorBeamsController;

    public static CollisionSystem CollisionSystem;

    public static SoundManager AudioManager;
    public static MusicManagerComponent MusicManager;

    public static readonly Dictionary<EnemyData, PrefabPool<EnemyComponent>> ENEMY_PREFAB_POOLS = new();
    
    public static readonly Dictionary<BulletComponent, PrefabPool<BulletComponent>> BULLET_PREFAB_POOLS = new();
    
    public static void Cleanup()
    {
        Data = null;
        IsGamePaused = false;
        UI = null;
        Player = null;
        Enemies = null;
        TractorBeamsController = null;
        CollisionSystem = null;
        AudioManager = null;
        MusicManager = null;
        
        ENEMY_PREFAB_POOLS.Clear();
        BULLET_PREFAB_POOLS.Clear();
        
        if (GameLoop.Instance)
        {
            GameLoop.Instance = null;
        }
    }

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
        return !enemyData ? null : ENEMY_PREFAB_POOLS[enemyData];
    }
    
    public static PrefabPool<BulletComponent> GetBulletPool(BulletComponent bullet)
    {
        if (BULLET_PREFAB_POOLS.TryGetValue(bullet, out var pool)) return pool;
        
        var poolHolder = new GameObject($"Pool_{bullet.name}");
        var newPool    = new PrefabPool<BulletComponent>(poolHolder, bullet, 100);
        BULLET_PREFAB_POOLS.Add(bullet, newPool);
        return newPool;
    }
}