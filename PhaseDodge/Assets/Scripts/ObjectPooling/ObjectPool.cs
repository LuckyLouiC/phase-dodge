using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private Dictionary<GameObject, List<GameObject>> pooledObjects = new Dictionary<GameObject, List<GameObject>>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetPooledObject(GameObject prefab)
    {
        if (!pooledObjects.ContainsKey(prefab))
        {
            pooledObjects[prefab] = new List<GameObject>();
        }

        if (pooledObjects[prefab].Count > 0)
        {
            GameObject obj = pooledObjects[prefab][pooledObjects[prefab].Count - 1];
            pooledObjects[prefab].RemoveAt(pooledObjects[prefab].Count - 1);
            obj.SetActive(true);
            return obj;
        }
        else
        {
            GameObject obj = Instantiate(prefab);
            return obj;
        }
    }

    public void ReturnToPool(GameObject prefab, GameObject obj)
    {
        obj.SetActive(false);
        pooledObjects[prefab].Add(obj);
    }
}