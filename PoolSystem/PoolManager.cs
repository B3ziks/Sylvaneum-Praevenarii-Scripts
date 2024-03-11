using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    [SerializeField] GameObject poolPrefab;
    Dictionary<int, ObjectPool> poolList;

    private void Awake()
    {

        // Your existing Awake code
        poolList = new Dictionary<int, ObjectPool>();
    }

    public void CreatePool(PoolObjectData newPoolData)
    {
        GameObject newObjectPoolGO = Instantiate(poolPrefab, transform).gameObject;
        ObjectPool newObjectPool = newObjectPoolGO.GetComponent<ObjectPool>();
        newObjectPool.Set(newPoolData);
        newObjectPoolGO.name = "Pool " +newPoolData.name;
        poolList.Add(newPoolData.poolID, newObjectPool);
    }

    public GameObject GetObject(PoolObjectData poolObjectData)
    {
        if (poolList.ContainsKey(poolObjectData.poolID) == false)
        {
            CreatePool(poolObjectData);
        }

        GameObject obj = poolList[poolObjectData.poolID].GetObject();

        // Activate the object when retrieved from the pool
        obj.SetActive(true);

        return obj;
    }

    // Method to deactivate all objects in all pools
    public void DeactivateAllPooledObjects()
    {
        foreach (var poolKey in poolList.Keys)
        {
            UnityEngine.Debug.Log($"Deactivating objects in pool with ID: {poolKey}");
            poolList[poolKey].DeactivateAllObjects();
        }
    }
}
