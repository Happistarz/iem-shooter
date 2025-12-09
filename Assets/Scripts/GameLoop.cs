using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLoop : MonoBehaviour
{
    //Wave state
    public int CurrentWaveIndex;
    public WaveParameters.Wave CurrentWave;
    public float WaveTimer;
    public bool IsPauseActive;

    //Game data
    private WaveParameters m_waveParameters;

    //current wave data 
    private Dictionary<WaveParameters.WavePart, EnemyData> m_selectedEnemies;
    private Dictionary<WaveParameters.WavePart, float> m_enemiesSpawnTimers;

    //References
    private UIManager m_uiManager;
    private PlayerComponent m_player;

    //Utils
    private System.Random m_random;

    public void Init()
    {
        m_waveParameters = Game.Data.WaveParameters;

        m_random = new System.Random();

        //Initialise Player
        var playerStart = GameObject.FindObjectOfType<PlayerStartLocation>();
        Vector3 startLocation = Vector3.zero;
        if (playerStart != null)
            startLocation = playerStart.transform.position;
        m_player = GameObject.Instantiate(Game.Data.PlayerPrefab);
        m_player.transform.position = startLocation;
        Game.Player = m_player;

        //Start the game
        StartWave(0);
        IsPauseActive = false;
    }

    public void Update()
    {
        if (IsPauseActive) return;

        WaveTimer += Time.deltaTime;
        foreach (var wavePart in m_enemiesSpawnTimers.Keys.ToList())
        {
            m_enemiesSpawnTimers[wavePart] += Time.deltaTime;

            var spawnDelay = m_waveParameters.WaveDuration / wavePart.Count / Game.Data.SpawnMultiplier;
            if (m_enemiesSpawnTimers[wavePart] > spawnDelay)
            {
                SpawnEnemy(m_selectedEnemies[wavePart]);
                m_enemiesSpawnTimers[wavePart] -= spawnDelay;
            }
        }

        if (WaveTimer > m_waveParameters.WaveDuration)
        {
            StartCoroutine("MoveToNextWaveCoroutine");
        }
    }

    void StartWave(int index)
    {
        CurrentWaveIndex = index;
        if (CurrentWaveIndex >= m_waveParameters.Waves.Count)
            CurrentWaveIndex = m_waveParameters.Waves.Count - 1;
        CurrentWave = m_waveParameters.Waves[CurrentWaveIndex];

        m_selectedEnemies = new Dictionary<WaveParameters.WavePart, EnemyData>();
        m_enemiesSpawnTimers = new Dictionary<WaveParameters.WavePart, float>();
        foreach (var wavePart in CurrentWave.Parts)
        {
            var enemiesAtThreat = Game.Data.Enemies.Where(e => e.Threat == wavePart.Threat).ToList();
            if (enemiesAtThreat.Count > 0)
            {
                var enemy = enemiesAtThreat[m_random.Next(0, enemiesAtThreat.Count)];
                m_selectedEnemies[wavePart] = enemy;
                m_enemiesSpawnTimers[wavePart] = 0;
            }
        }

        WaveTimer = 0;
    }

    void SpawnEnemy(EnemyData enemyData)
    {
        var spawnLocations = GameObject.FindObjectsOfType<SpawnLocationComponent>();
        var spawner = spawnLocations[m_random.Next(0, spawnLocations.Length)];
        EnemyComponent enemy = GameObject.Instantiate(enemyData.Prefab);
        enemy.transform.position = spawner.transform.position;
    }

    IEnumerator MoveToNextWaveCoroutine()
    {
        IsPauseActive = true;

        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(0.5f);

        m_uiManager = GameObject.FindObjectOfType<UIManager>();
        m_uiManager.ShowTitle();
        yield return new WaitForSecondsRealtime(0.5f);

        m_uiManager.ShowNoUpgradeText();
        yield return new WaitForSecondsRealtime(2.0f);

        m_uiManager.HideAll();

        Time.timeScale = 1;
        StartWave(CurrentWaveIndex + 1);
        IsPauseActive = false;
    }
}