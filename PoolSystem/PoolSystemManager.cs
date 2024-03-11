using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolSystemManager : MonoBehaviour
{
    // Call this method to deactivate all children of each child pool.
    public void DeactivateAllPooledObjects()
    {
        foreach (Transform pool in transform)
        {
            // Now iterate over each pool's children and deactivate them.
            foreach (Transform poolObject in pool)
            {
                poolObject.gameObject.SetActive(false);
            }
        }
    }

    // Call this method to reactivate the pool system, assuming you want to make them all active again.

}