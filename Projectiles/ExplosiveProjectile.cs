using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ExplosiveProjectile : Projectile
{
    [SerializeField]
    private PoolObjectData explosionEffectPoolData;

    public float explosionRadius; // radius of explosion

    private Vector3 targetPosition;
    private PoolManager poolManager;

    // This is already present in Projectile. We just need to override it.
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

    protected void Update()
    {
        base.Update();

        if (Vector3.Distance(transform.position, targetPosition) <= 0.1f)
        {
            Explode();
        }
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
                // Instead of destroying the grenade when it hits an enemy,
                // we explode it, dealing damage to all enemies in the radius.
                if (!hasExploded)
                {
                    Explode();
                    hasExploded = true;
                }
            }
        }
    }

    private void SetTargetPosition()
    {
        targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetPosition.z = 0;
    }

    private void Explode()
    {
        // Damage and post damage to enemies
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D c in hit)
        {
            IDamageable enemy = c.GetComponent<IDamageable>();
            if (enemy != null && !CheckRepeatHit(enemy)) // Reuse the CheckRepeatHit method from base class
            {
                weapon.ApplyDamage(c.transform.position, damage, enemy);
                enemiesHit.Add(enemy); // Assuming `enemiesHit` is protected in base class
                PostDamage(damage, c.transform.position);
            }
        }

        // Spawn explosion effect
        if (poolManager && explosionEffectPoolData)
        {
            GameObject explosionEffect = poolManager.GetObject(explosionEffectPoolData);
            if (explosionEffect)
            {
                explosionEffect.transform.position = transform.position;
                explosionEffect.transform.rotation = Quaternion.identity;
                explosionEffect.transform.localScale = Vector3.one * explosionRadius * 2;  // Scaling to the diameter
            }
            else
            {
                UnityEngine.Debug.LogError("Failed to get explosion effect from pool.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("PoolManager or explosionEffectPoolData is not set.");
        }

        DestroyProjectile();
    }

    public void SetPoolManager(PoolManager poolManager)
    {
        this.poolManager = poolManager;
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