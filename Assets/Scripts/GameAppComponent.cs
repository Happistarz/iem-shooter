using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class GameAppComponent : MonoBehaviour
{
    private GameLoop _gameLoop;
    
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
        _gameLoop = gameLoopObject.AddComponent<GameLoop>();
        _gameLoop.Init();
    }

    private void Update()
    {
        _gameLoop.Update();
    }
}