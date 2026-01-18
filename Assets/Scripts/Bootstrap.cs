using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameApp
{
    private const string _GAME_PERSISTENT_ROOT_NAME = "GamePersistentRoot";

    private static GameObject PersistentRoot { get; set; }
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Menu" && mode == LoadSceneMode.Single)
        {
            CleanupPersistentObjects();
        }
    }
    
    public static GameObject GetPersistentRoot()
    {
        if (PersistentRoot) return PersistentRoot;
        
        PersistentRoot = GameObject.Find(_GAME_PERSISTENT_ROOT_NAME);
        if (PersistentRoot) return PersistentRoot;
        
        PersistentRoot = new GameObject(_GAME_PERSISTENT_ROOT_NAME);
        Object.DontDestroyOnLoad(PersistentRoot);

        return PersistentRoot;
    }
    
    public static void CleanupPersistentObjects()
    {
        Game.Cleanup();
        
        if (PersistentRoot == null) return;
        
        Object.Destroy(PersistentRoot);
        PersistentRoot = null;
    }
}