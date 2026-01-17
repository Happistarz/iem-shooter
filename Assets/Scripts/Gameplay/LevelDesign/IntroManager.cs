using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    private GameAppComponent _gameApp;
    public string gameSceneName = "Niveau 1";
    
    private bool _transitionStarted = false;
    
    private void Start()
    {
        _gameApp = FindFirstObjectByType<GameAppComponent>();
        
        if (!_gameApp)
            Debug.LogError("GameAppComponent not found in the scene.");
    }
    
    public void StartGameTransition()
    {
        if (_transitionStarted) return;
        
        _transitionStarted = true;
        StartCoroutine(LoadGameSceneCoroutine());
    }
    
    private IEnumerator LoadGameSceneCoroutine()
    {
        DontDestroyOnLoad(_gameApp.gameObject);
        
        var asyncLoad = SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Additive);
        while (asyncLoad is { isDone: false })
        {
            yield return null;
        }
        
        SceneManager.UnloadSceneAsync("Intro");
        
        _gameApp.InitializeGame();
    }
}