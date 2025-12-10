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

    //current wave data 
    private Dictionary<WaveParameters.WavePart, List<EnemyData>> _availableEnemies;
    private Dictionary<WaveParameters.WavePart, float>           _enemiesSpawnTimers;

    //References
    private UIManager       _uiManager;
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
        StartWave(0);
        isPauseActive = false;

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

        waveTimer += Time.deltaTime;
        foreach (var wavePart in _enemiesSpawnTimers.Keys.ToList())
        {
            _enemiesSpawnTimers[wavePart] += Time.deltaTime;

            var spawnDelay = _waveParameters.WaveDuration / wavePart.Count / Game.Data.SpawnMultiplier;
            if (!(_enemiesSpawnTimers[wavePart] > spawnDelay)) continue;

            var enemyToSpawn = SelectRandomEnemy(_availableEnemies[wavePart]);
            SpawnEnemy(enemyToSpawn);
            _enemiesSpawnTimers[wavePart] -= spawnDelay;
        }

        if (waveTimer > _waveParameters.WaveDuration)
        {
            StartCoroutine(nameof(MoveToNextWaveCoroutine));
        }
    }

    private void StartWave(int index)
    {
        currentWaveIndex = index;
        if (currentWaveIndex >= _waveParameters.Waves.Count)
            currentWaveIndex = _waveParameters.Waves.Count - 1;
        currentWave = _waveParameters.Waves[currentWaveIndex];

        _availableEnemies   = new Dictionary<WaveParameters.WavePart, List<EnemyData>>();
        _enemiesSpawnTimers = new Dictionary<WaveParameters.WavePart, float>();
        foreach (var wavePart in currentWave.Parts)
        {
            var enemiesAtThreat = Game.Data.Enemies.Where(e => e.Threat == wavePart.Threat).ToList();
            if (enemiesAtThreat.Count <= 0) continue;

            _availableEnemies[wavePart]   = enemiesAtThreat;
            _enemiesSpawnTimers[wavePart] = 0;
        }

        waveTimer = 0;
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
        var enemy = Game.ENEMY_PREFAB_POOLS[enemyData].Get();

        enemy.enemyData          = enemyData;
        enemy.transform.position = spawner.transform.position;
        enemy.health             = enemyData.Health;
    }

    private IEnumerator MoveToNextWaveCoroutine()
    {
        isPauseActive = true;

        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.5f);

        _uiManager = FindFirstObjectByType<UIManager>();
        _uiManager.ShowTitle();
        yield return new WaitForSecondsRealtime(0.5f);

        _uiManager.ShowNoUpgradeText();
        yield return new WaitForSecondsRealtime(2.0f);

        _uiManager.HideAll();

        Time.timeScale = 1;
        StartWave(currentWaveIndex + 1);
        isPauseActive = false;
    }
}