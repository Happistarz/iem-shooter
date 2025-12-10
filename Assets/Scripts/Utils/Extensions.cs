using UnityEngine;

public static class Extensions
{
    public static T GetComponentInSelfOrChildren<T>(this MonoBehaviour monoBehaviour) where T : Component
    {
        return monoBehaviour.gameObject.GetComponentInSelfOrChildren<T>();
    }
    
    public static T GetComponentInSelfOrChildren<T>(this GameObject gameObject) where T : Component
    {
        var component = gameObject.GetComponent<T>();
        if (component != null)
        {
            return component;
        }

        foreach (Transform child in gameObject.transform)
        {
            component = child.gameObject.GetComponentInSelfOrChildren<T>();
            if (component != null)
            {
                return component;
            }
        }

        return null;
    }
}