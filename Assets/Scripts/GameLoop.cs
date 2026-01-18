using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class GameLoop : MonoBehaviour
{
    // Singleton
    public static GameLoop Instance { get; set; }
    
    //Wave state
    [FormerlySerializedAs("CurrentWaveIndex")]
    public int currentWaveIndex;

    [FormerlySerializedAs("CurrentWave")] public WaveParameters.Wave currentWave;

    [FormerlySerializedAs("WaveTimer")] public float waveTimer;

    //Game data
    private WaveParameters _waveParameters;

    private Queue<EnemyData> _enemiesToSpawnQueue = new();
    private float            _spawnTimer;
    private float            _spawnInterval = 1.0f;

    //References
    private PlayerComponent       _player;
    public  CameraSystemComponent cameraSystem;
    public  BossFightComponent    bossFight;

    //Utils
    private System.Random _random;

    private bool _isWaveTransitioning;
    private bool _isGameActive;
    private bool _inBossFight;

    public void Init()
    {
        //Singleton setup
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        Game.Enemies    = new List<EnemyComponent>();
        _waveParameters = Game.Data.WaveParameters;

        _random = new System.Random();

        //Initialise Player
        var playerStart   = FindFirstObjectByType<PlayerStartLocation>();
        var startLocation = Vector3.zero;
        if (playerStart)
            startLocation = playerStart.transform.position;
        _player                    = Instantiate(Game.Data.PlayerPrefab);
        _player.transform.position = startLocation;
        Game.Player                = _player;

        //Initialize Camera System
        cameraSystem = FindFirstObjectByType<CameraSystemComponent>();
        if (!cameraSystem) Debug.LogError("No camera found");

        bossFight = FindFirstObjectByType<BossFightComponent>(FindObjectsInactive.Include);
        if (!bossFight) Debug.LogError("No boss fight component found");
        
        //Start the game
        Game.MusicManager.PlayWaveMusic();
        StartCoroutine(StartGame());

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
    }

    private IEnumerator StartGame()
    {
        var duration = Game.TractorBeamsController.ActivateBeams();
        yield return new WaitForSeconds(duration);
        StartWave(0);
        _isGameActive = true;
    }

    public void Update()
    {
        if (Game.IsGamePaused || !_isGameActive) return;

        Game.CollisionSystem.UpdateGrid();
        
        if (_inBossFight) return;

        waveTimer   += Time.deltaTime;
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnInterval && _enemiesToSpawnQueue.Count > 0)
        {
            _spawnTimer = 0;
            var enemyData = _enemiesToSpawnQueue.Dequeue();
            SpawnEnemy(enemyData);
        }

        // Next wave
        if (_enemiesToSpawnQueue.Count > 0 || Game.Enemies.Count > 0) return;

        if (_isWaveTransitioning) return;
        _isWaveTransitioning = true;
        StartCoroutine(HandleWaveCompleted());
    }

    private IEnumerator HandleWaveCompleted()
    {
        Game.IsGamePaused = true;

        yield return new WaitForSeconds(1.0f);

        Game.UI.ShowBossReaction(currentWave.BossReaction, () =>
        {
            _isWaveTransitioning = false;
            Game.IsGamePaused    = false;

            StartWave(currentWaveIndex + 1);
        });
    }

    private void StartWave(int index)
    {
        currentWaveIndex = index;

        if (currentWaveIndex >= _waveParameters.Waves.Count)
        {
            StartCoroutine(StartBossFight());
            return;
        }

        currentWave = _waveParameters.Waves[currentWaveIndex];
        HandleBeamMovement(currentWave.MoveBeamChance, currentWave.BeamIndex);

        waveTimer   = 0;
        _spawnTimer = 0;

        _spawnInterval = currentWave.Duration / currentWave.TotalEnemies;

        PrepareWaveEnemies();
    }

    private void HandleBeamMovement(int chance, int? beamIndex)
    {
        if (_random.Next(0, 100) >= chance) return;

        if (beamIndex < 0) beamIndex = null;
        Game.TractorBeamsController.MoveRandomBeams(beamIndex);
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

    private IEnumerator StartBossFight()
    {
        Game.IsGamePaused = true;
        _inBossFight      = true;
        
        Game.MusicManager.FadeToVolume(Game.MusicManager.bossVolume, 1.0f);
        Game.MusicManager.PlayBossMusic();
        yield return new WaitForSeconds(2.0f);

        HandleBeamMovement(100, 0);

        yield return new WaitForSeconds(2.0f);

        cameraSystem.ShakeCamera(1.0f, 1.5f);
        Game.TractorBeamsController.ChangeBeamsColors();

        var terrainCorruption = FindFirstObjectByType<TerrainCorruptionComponent>();
        if (terrainCorruption)
        {
            terrainCorruption.CorruptTerrain();
            yield return new WaitForSeconds(terrainCorruption.duration / 3);
        }

        var treeDissolver = FindFirstObjectByType<TreeDissolveComponent>();
        if (treeDissolver)
        {
            treeDissolver.DissolveTrees();
            yield return new WaitForSeconds(2.0f);
        }

        yield return new WaitForSeconds(Game.TractorBeamsController.ActivateBossBeam());

        bossFight.transform.position = Game.TractorBeamsController.bossBeam.location.transform.position;
        bossFight.gameObject.SetActive(true);
        
        // Wait for the boss fight intro animation
        yield return new WaitForSeconds(2.0f);
        Game.TractorBeamsController.bossBeam.FadeOut();

        Game.IsGamePaused = false;
    }
    
    public void OnBossDefeated()
    {
        Game.IsGamePaused = true;
        _isGameActive    = false;

        StartCoroutine(HandleGameCompleted(true));
    }
    
    public void OnPlayerDefeated()
    {
        Game.IsGamePaused = true;
        _isGameActive    = false;

        StartCoroutine(HandleGameCompleted(false));
    }
    
    private static IEnumerator HandleGameCompleted(bool win)
    {
        if (win)
        {
            // clean up enemies
            foreach (var enemy in Game.Enemies.ToList())
            {
                enemy.ApplyDamage(enemy.health);
            }
            
            yield return new WaitForSeconds(3.0f);

            Game.TractorBeamsController.DeactivateBeams();
            var terrainCorruption = FindFirstObjectByType<TerrainCorruptionComponent>();
            if (terrainCorruption)
            {
                terrainCorruption.RestoreTerrain();
            }
        } else
        {
            yield return new WaitForSeconds(2.0f);
        }
        
        Game.UI.ShowGameOver(win);
        
        Game.MusicManager.FadeOutMusic(2.0f);
    }

    public void SpawnBossEnemy()
    {
        var simpleEnemies = Game.Data.Enemies
                                .Where(enemy => enemy.Threat.Equals(EnemyData.ThreatLevel.Advanced))
                                .ToList();
        if (simpleEnemies.Count == 0) return;

        var enemyData = SelectRandomEnemy(simpleEnemies);
        SpawnEnemy(enemyData);
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
        var spawner = Game.TractorBeamsController.GetRandomActiveBeamLocation();
        var enemy   = Game.ENEMY_PREFAB_POOLS[enemyData].Get();
        Game.Enemies.Add(enemy);

        enemy.enemyData          = enemyData;
        enemy.transform.position = spawner.GetSpawnPosition();
        enemy.health             = enemyData.Health;
    }
}