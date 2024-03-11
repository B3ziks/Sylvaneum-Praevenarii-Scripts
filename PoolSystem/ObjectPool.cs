using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    PoolObjectData originalPoolData;
    List<GameObject> pool;

    public void Set(PoolObjectData pod)
    {
        pool = new List<GameObject>();
        originalPoolData = pod;
    }

    public void InstantiateObject()
    {
        GameObject newObject = Instantiate(originalPoolData.originalPrefab, transform);
        GameObject mainObject = newObject;

        if (originalPoolData.containerPrefab != null)
        {
            GameObject container = Instantiate(originalPoolData.containerPrefab);
            newObject.transform.SetParent(container.transform);
            newObject.transform.localPosition = Vector3.zero;
            mainObject = container;
        }

        pool.Add(mainObject);
        PoolMember poolMember = mainObject.AddComponent<PoolMember>();
        poolMember.Set(this);
        mainObject.SetActive(false); // Start with the object deactivated
    }

    public GameObject GetObject()
    {
        for (int i = pool.Count - 1; i >= 0; i--)
        {
            if (!pool[i].activeInHierarchy)
            {
                GameObject obj = pool[i];
                pool.RemoveAt(i); // Remove from the pool list
                obj.SetActive(true);
                return obj;
            }
        }

        // If no inactive object is found, instantiate a new one
        InstantiateObject();
        GameObject newObj = pool[pool.Count - 1];
        pool.RemoveAt(pool.Count - 1);
        newObj.SetActive(true);
        return newObj;
    }

    public void ReturnToPool(GameObject gameObject)
    {
        if (!pool.Contains(gameObject)) // Prevent duplicates
        {
            gameObject.SetActive(false);
            pool.Add(gameObject);
        }
        else
        {
            return;
           //UnityEngine.Debug.LogWarning("Attempted to return an object to the pool that is already in the pool: " + gameObject.name);
        }
    }
    // Method to deactivate all objects in this pool
    public void DeactivateAllObjects()
    {
        foreach (GameObject obj in pool)
        {
            if (obj.activeInHierarchy)
            {
                obj.SetActive(false);
            }
        }
    }
}