using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;


public class RocketTurretProjectile : TurretProjectile
{
    [SerializeField]
    private PoolObjectData explosionEffectPoolData; // Data for the explosion effect prefab

    public float explosionRadius; // Radius of explosion

    private PoolManager poolManager;

    protected void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void SetStats(WeaponBase weaponBase)
    {
        base.SetStats(weaponBase);
        explosionRadius = weaponBase.weaponData.stats.explosionRadius;
    }

    protected override void DetectHits()
    {
        base.DetectHits(); // Call the base detection logic

        // After the base logic, check for explosion conditions
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackArea);
        foreach (Collider2D collider in hitColliders)
        {
            IDamageable enemy = collider.GetComponent<IDamageable>();
            if (enemy != null && !AlreadyHit(enemy))
            {
                Explode();
                break; // Ensure that explode is only called once per update
            }
        }
    }

    private void Explode()
    {
        // Perform explosion logic
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D collider in hitColliders)
        {
            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null && !AlreadyHit(damageable))
            {
                weapon.ApplyDamage(collider.transform.position, damage, damageable);
                enemiesHit.Add(damageable);
                DisplayDamage(damage, collider.transform.position);
            }
        }

        // Spawn explosion effect
        if (poolManager != null && explosionEffectPoolData != null)
        {
            GameObject explosionEffect = poolManager.GetObject(explosionEffectPoolData);
            if (explosionEffect)
            {
                explosionEffect.transform.position = transform.position;
                explosionEffect.transform.rotation = Quaternion.identity;
                explosionEffect.transform.localScale = Vector3.one * (explosionRadius * 2); // Scaling to the diameter
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

        DeactivateProjectile();
    }

    // If your TurretProjectile class doesn't already have OnTriggerEnter2D, add it
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other); // Call the base method to handle other collisions

        // Specific behavior for the rocket when hitting an obstacle or enemy
        if (other.CompareTag("Obstacle") || other.CompareTag("Enemy"))
        {
            Explode();
        }
    }
}