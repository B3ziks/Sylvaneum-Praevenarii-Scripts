using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShamanTotem : MonoBehaviour
{
    public float damageRadius = 3f;
    public float damageInterval = 1f;
    public int damageAmount = 10;

    private Transform playerTransform;
    private float lifetime;
    private float damageCountdown;
    private PoolManager poolManager;
    private PoolObjectData poolData;

    public void Initialize(Transform targetPlayer, float lifetime, PoolManager poolManager, PoolObjectData poolData)
    {
        this.playerTransform = targetPlayer;
        this.lifetime = lifetime;
        this.poolManager = poolManager;
        this.poolData = poolData;
        damageCountdown = damageInterval;
    }

    private void Update()
    {
        if (playerTransform == null) return;

        damageCountdown -= Time.deltaTime;
        if (damageCountdown <= 0f)
        {
            DamagePlayer();
            damageCountdown = damageInterval;
        }

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            ReturnToPool();
        }
    }

    private void DamagePlayer()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) <= damageRadius)
        {
            IDamageable playerDamageable = playerTransform.GetComponent<IDamageable>();
            if (playerDamageable != null)
            {
                playerDamageable.TakeDamage(damageAmount);
            }
        }
    }

    private void ReturnToPool()
    {
        gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
}