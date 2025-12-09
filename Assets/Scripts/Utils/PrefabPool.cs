using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PrefabPool
{
    List<GameObject> m_availableObjects;
    List<GameObject> m_usedObjects;

    private GameObject m_prefab;

    public PrefabPool(GameObject prefab)
    {
        m_availableObjects = new List<GameObject>();
        m_usedObjects = new List<GameObject>();
        m_prefab = prefab;
    }

    public GameObject Get()
    {
        if (m_availableObjects.Count == 0)
        {
            GameObject newInstance = GameObject.Instantiate(m_prefab);
            m_availableObjects.Add(newInstance);
        }

        GameObject instance = m_availableObjects.Last();
        m_availableObjects.RemoveAt(m_availableObjects.Count - 1);
        m_usedObjects.Add(instance);

        instance.SetActive(true);
        return instance;
    }

    public void Release(GameObject gameObject)
    {
        m_availableObjects.Add(gameObject);
        m_usedObjects.Remove(gameObject);

        gameObject.SetActive(false);
    }
}

public class PrefabPool<T> where T : MonoBehaviour
{
    List<T> m_availableObjects;
    List<T> m_usedObjects;

    private T m_prefab;
    private GameObject m_instanceRoot;

    public PrefabPool(GameObject instanceRoot, T prefab, int minInstanceCount = 0)
    {
        m_availableObjects = new List<T>();
        m_usedObjects = new List<T>();

        m_prefab = prefab;
        m_instanceRoot = instanceRoot;

        for (int i = 0; i < minInstanceCount; i++)
            CreateInstance();
    }

    public T Get()
    {
        if (m_availableObjects.Count == 0)
            CreateInstance();

        T instance = m_availableObjects.Last();
        m_availableObjects.RemoveAt(m_availableObjects.Count - 1);
        m_usedObjects.Add(instance);

        instance.gameObject.SetActive(true);
        return instance;
    }

    private void CreateInstance()
    {
        T newInstance = GameObject.Instantiate(m_prefab, m_instanceRoot.transform);
        newInstance.gameObject.SetActive(false);
        m_availableObjects.Add(newInstance);
    }

    public void Release(T instance)
    {
        m_availableObjects.Add(instance);
        m_usedObjects.Remove(instance);

        instance.gameObject.SetActive(false);
    }
}