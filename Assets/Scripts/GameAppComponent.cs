using System.Collections.Generic;
using DG.Tweening;
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
        Game.CollisionSystem = new CollisionSystem(Game.Data.GridExtent, Game.Data.GridCellCount);

        DOTween.Init(false, true, LogBehaviour.ErrorsOnly);
        DOTween.SetTweensCapacity(2000, 500);
        
        var audioManagerObject = new GameObject("AudioManager");
        Game.AudioManager = audioManagerObject.AddComponent<SoundManager>();
        Game.AudioManager.Init();

        var gameLoopObject = new GameObject("GameLoop");
        _gameLoop = gameLoopObject.AddComponent<GameLoop>();
        _gameLoop.Init();
    }

    private void Update()
    {
        _gameLoop.Update();
    }
}