using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GasGrenadeProjectile : Projectile
{
    [SerializeField]
    private PoolObjectData gasCloudPoolData; // Pool data for the gas cloud effect

    public float explosionRadius; // Radius of gas cloud explosion

    private Vector3 targetPosition;
    private PoolManager poolManager;
    public int cloudDamagePerTick;
    public float cloudDuration;

    public override void SetStats(WeaponBase weaponBase)
    {
        base.SetStats(weaponBase);
        explosionRadius = weaponBase.weaponData.stats.explosionRadius;
    }

    protected void Awake()
    {
        poolManager = FindObjectOfType<PoolManager>();
        SetTargetPosition();
    }

    protected override void Update()
    {
        base.Update();

        if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
        {
            Explode();
        }
    }
    public void SetGasCloudStats(float duration, int damagePerTick)
    {
        cloudDuration = duration;
        cloudDamagePerTick = damagePerTick;
    }
    private void SetTargetPosition()
    {
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0;
    }

    public override void HitDetection()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, attackArea);
        bool hasExploded = false;
        foreach (Collider2D c in hit)
        {
            IDamageable enemy = c.GetComponent<IDamageable>();
            if (enemy != null && !CheckRepeatHit(enemy))
            {
                if (!hasExploded)
                {
                    Explode();
                    hasExploded = true;
                }
            }
        }
    }

    private void Explode()
    {
        // Damage and post damage to enemies
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D c in hit)
        {
            IDamageable enemy = c.GetComponent<IDamageable>();
            if (enemy != null && !CheckRepeatHit(enemy))
            {
                weapon.ApplyDamage(c.transform.position, damage, enemy);
                enemiesHit.Add(enemy);
                PostDamage(damage, c.transform.position);
            }
        }

        // Spawn gas cloud effect
        if (poolManager && gasCloudPoolData)
        {
            GameObject gasCloud = poolManager.GetObject(gasCloudPoolData);
            if (gasCloud)
            {
                gasCloud.transform.position = transform.position;
                gasCloud.transform.rotation = Quaternion.identity;
                PoisonOnEnemy gasCloudScript = gasCloud.GetComponent<PoisonOnEnemy>();
                if (gasCloudScript != null)
                {
                    gasCloudScript.damage = cloudDamagePerTick;
                    gasCloudScript.poisonDuration = cloudDuration;
                }
            }
            else
            {
                UnityEngine.Debug.LogError("Failed to get gas cloud from pool.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("PoolManager or gasCloudPoolData is not set.");
        }

        DestroyProjectile();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.CompareTag("Obstacle") || other.CompareTag("Enemy"))
        {
            Explode();
        }
    }
}