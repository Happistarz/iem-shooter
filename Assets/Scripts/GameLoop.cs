using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class GameLoop : MonoBehaviour
{
    //Wave state
    [FormerlySerializedAs("CurrentWaveIndex")]
    public int currentWaveIndex;

    [FormerlySerializedAs("CurrentWave")] public WaveParameters.Wave currentWave;
    [FormerlySerializedAs("WaveTimer")]   public float               waveTimer;

    [FormerlySerializedAs("IsPauseActive")]
    public bool isPauseActive;

    //Game data
    private WaveParameters _waveParameters;

    private Queue<EnemyData> _enemiesToSpawnQueue = new();
    private float            _spawnTimer;
    private float            _spawnInterval = 1.0f;

    //References
    private PlayerComponent _player;

    //Utils
    private System.Random _random;

    public void Init()
    {
        _waveParameters = Game.Data.WaveParameters;

        _random = new System.Random();

        //Initialise Player
        var playerStart   = FindFirstObjectByType<PlayerStartLocation>();
        var startLocation = Vector3.zero;
        if (playerStart != null)
            startLocation = playerStart.transform.position;
        _player                    = Instantiate(Game.Data.PlayerPrefab);
        _player.transform.position = startLocation;
        Game.Player                = _player;

        //Start the game
        // StartWave(0);
        isPauseActive = true; // DEBUG: Start paused for testing

        //Initialize prefab pools
        foreach (var enemyData in Game.Data.Enemies)
        {
            var poolHolder = new GameObject($"Pool_{enemyData.Prefab.name}");
            Game.ENEMY_PREFAB_POOLS.Add(
                enemyData,
                new PrefabPool<EnemyComponent>(
                    poolHolder,
                    enemyData.Prefab,
                    minInstanceCount: 200));
        }

        //Initialize bullet pool
        var bulletPoolHolder = new GameObject("Pool_Bullets");
        Game.BulletPrefabPool = new PrefabPool<BulletComponent>(
            bulletPoolHolder,
            Game.Data.BulletPrefab,
            minInstanceCount: 200);
    }

    public void Update()
    {
        if (isPauseActive) return;

        Game.CollisionSystem.UpdateGrid();

        waveTimer += Time.deltaTime;
        _spawnTimer += Time.deltaTime;
        
        if (_spawnTimer >= _spawnInterval && _enemiesToSpawnQueue.Count > 0)
        {
            _spawnTimer = 0;
            var enemyData = _enemiesToSpawnQueue.Dequeue();
            SpawnEnemy(enemyData);
        }

        // Next wave
        if (_enemiesToSpawnQueue.Count <= 0 || Game.Enemies.Count <= 0 || currentWave == null) return;
        StartCoroutine(nameof(MoveToNextWaveCoroutine));
    }

    private void StartWave(int index)
    {
        currentWaveIndex = index;

        if (currentWaveIndex >= _waveParameters.Waves.Count)
        {
            StartBossFight();
            return;
        }
        
        currentWave = _waveParameters.Waves[currentWaveIndex];
        waveTimer  = 0;
        _spawnTimer = 0;

        _spawnInterval = currentWave.Duration / currentWave.TotalEnemies;
        
        PrepareWaveEnemies();
    }

    private void PrepareWaveEnemies()
    {
        _enemiesToSpawnQueue.Clear();
        List<EnemyData> waveList = new();

        foreach (var part in currentWave.Parts)
        {
            var enemiesOfType = Game.Data.Enemies
                .Where(enemy => enemy.Threat.Equals(part.Threat))
                .ToList();
            if (enemiesOfType.Count == 0) continue;
            
            var count = Mathf.RoundToInt(currentWave.TotalEnemies * (part.Percentage / 100.0f));
            for (var i = 0; i < count; i++)
            {
                waveList.Add(SelectRandomEnemy(enemiesOfType));
            }
        }

        while (waveList.Count < currentWave.TotalEnemies)
        {
            var simpleEnemies = Game.Data.Enemies
                .Where(enemy => enemy.Threat.Equals(EnemyData.ThreatLevel.Simple))
                .ToList();
            waveList.Add(SelectRandomEnemy(simpleEnemies));
        }
        
        var shuffledWaveList = waveList.OrderBy(_ => _random.Next()).ToList();
        
        _enemiesToSpawnQueue = new Queue<EnemyData>(shuffledWaveList);
    }
    
    private void StartBossFight()
    {
        isPauseActive = true;
        
        Debug.Log("Boss Fight Started!");
    }
    
    private EnemyData SelectRandomEnemy(List<EnemyData> enemies)
    {
        // Calcule le poids total
        var totalWeight = enemies.Sum(enemy => enemy.Rarity);

        // Sélection pondérée
        var randomValue   = _random.Next(0, totalWeight);
        var currentWeight = 0;

        foreach (var enemy in enemies)
        {
            currentWeight += enemy.Rarity;
            if (randomValue < currentWeight)
                return enemy;
        }

        return enemies[0];
    }

    private void SpawnEnemy(EnemyData enemyData)
    {
        var spawnLocations = FindObjectsByType<SpawnLocationComponent>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        var spawner = spawnLocations[_random.Next(0, spawnLocations.Length)];
        var enemy   = Game.ENEMY_PREFAB_POOLS[enemyData].Get();

        enemy.enemyData          = enemyData;
        enemy.transform.position = spawner.GetSpawnPosition();
        enemy.health             = enemyData.Health;
    }

    private IEnumerator MoveToNextWaveCoroutine()
    {
        isPauseActive = true;

        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.5f);

        Game.UI.ShowTitle();
        yield return new WaitForSecondsRealtime(0.5f);

        Game.UI.ShowNoUpgradeText();
        yield return new WaitForSecondsRealtime(2.0f);

        Game.UI.HideAll();

        Time.timeScale = 1;
        StartWave(currentWaveIndex + 1);
        isPauseActive = false;
    }
}