using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class GameAppComponent : MonoBehaviour
{
    private GameLoop m_gameLoop;
    
    public void Start()
    {
        DontDestroyOnLoad(gameObject);
        
        SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        
        var gameData = Addressables.LoadAssetAsync<GameData>("Assets/Data/GameData.asset").WaitForCompletion();
        
        Game.Data = gameData;
        Game.Enemies = new List<EnemyComponent>();
        Game.Player = null;
        Game.CollisionSystem = new CollisionSystem();

        var gameLoopObject = new GameObject("GameLoop");
        m_gameLoop = gameLoopObject.AddComponent<GameLoop>();
        m_gameLoop.Init();
    }
    
    void Update()
    {
        m_gameLoop.Update();
    }
}