using UnityEngine;

public static class GameApp
{
    [RuntimeInitializeOnLoadMethod]
    private static void Bootstrap()
    {
        var gameAppObject = new GameObject("Game");
        gameAppObject.AddComponent<GameAppComponent>();
    }
}