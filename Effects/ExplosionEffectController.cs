using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffectController : MonoBehaviour
{
    private float effectDuration = 2f;  // The time after which the effect will be returned to the pool
    PoolMember poolMember;

    private void OnEnable()
    {
        StartCoroutine(ReturnEffectToPoolAfterDelay());
    }

    private IEnumerator ReturnEffectToPoolAfterDelay()
    {
        yield return new WaitForSeconds(effectDuration);

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (poolMember == null)
        {
            poolMember = GetComponent<PoolMember>();
        }

        if (poolMember != null)
        {
            poolMember.ReturnToPool();
        }
        else
        {
            UnityEngine.Debug.LogError("PoolMember not found on ExplosionEffect");
        }
    }
}