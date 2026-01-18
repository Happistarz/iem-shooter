using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    private GameAppComponent _gameApp;
    public string gameSceneName = "Niveau 1";
    
    private bool _transitionStarted;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ReturnToMenu();
        }
    }
    
    public void StartGameTransition()
    {
        if (_transitionStarted) return;
        
        _transitionStarted = true;
        StartCoroutine(LoadGameSceneCoroutine());
    }
    
    private IEnumerator LoadGameSceneCoroutine()
    {
        var gameAppObject = new GameObject("GameApp");
        _gameApp = gameAppObject.AddComponent<GameAppComponent>();
        
        var asyncLoad = SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Additive);
        while (asyncLoad is { isDone: false })
        {
            yield return null;
        }
        
        SceneManager.UnloadSceneAsync("Intro");
        
        _gameApp.InitializeGame();
    }
    
    private void ReturnToMenu()
    {
        StopAllCoroutines();
        
        DOTween.KillAll();
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}