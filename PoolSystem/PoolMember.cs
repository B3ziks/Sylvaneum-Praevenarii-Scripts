using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMember : MonoBehaviour, IPoolMember
{
    ObjectPool pool;
    PoolMember poolMember;
    public void Set(ObjectPool pool)
    {
        // Check if pool is null before trying to use it
        if (pool == null)
        {
            UnityEngine.Debug.LogError("Attempted to set a null pool in PoolMember.Set");
            return;
        }

        this.pool = pool;

        // Ensure GetComponent<IPoolMember>() and this method aren’t returning/setting null
        IPoolMember iPoolMember = GetComponent<IPoolMember>();
        if (iPoolMember == null)
        {
            UnityEngine.Debug.LogError("IPoolMember component not found in " + gameObject.name);
            return;
        }

        iPoolMember.SetPoolMember(this);
    }

    public void ReturnToPool()
    {
        // Check if pool is null before trying to use it
        if (pool == null)
        {
            UnityEngine.Debug.LogError("Attempted to use a null pool in PoolMember.ReturnToPool");
            return;
        }

        // Deactivate the object when returned to pool
        gameObject.SetActive(false);
        pool.ReturnToPool(gameObject);
    }

    public void SetPoolMember(PoolMember poolMember)
    {
        this.poolMember = poolMember;
    }
}