using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemyRanged : Enemy, IPoolMember, IDamageable
{
    private float shootingInterval => enemyData.stats.timeToAttack;
    private float shootingTimer;
    PoolManager poolManager;
    protected virtual void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();
      //  UnityEngine.Debug.Log($"Enemy initialized with Elemental Potency: {enemyData.stats.elementalPotency} and DOT: {enemyData.stats.elementalDamageOverTime}");

        // Initialize your ranged enemy logic here
    }

    protected virtual void Update()
    {
        HandleShooting();
    }

    private void HandleShooting()
    {
        shootingTimer += Time.deltaTime;
        if (shootingTimer >= shootingInterval)
        {
            shootingTimer -= shootingInterval; // Preserve any extra time.
            ShootProjectile();
        }
    }

    protected void ShootProjectile()
    {
        Vector3 spawnPosition = transform.position;
        if (enemyData.projectilePoolData?.originalPrefab != null)
        {
            GameObject projectile = poolManager.GetObject(enemyData.projectilePoolData);
            if (projectile != null)
            {
                projectile.transform.position = spawnPosition;
                EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();
                if (enemyProjectile != null)
                {
                    enemyProjectile.damage = stats.damage;
                    enemyProjectile.shootingEnemyData = this.enemyData;

                    // Set elementalPotency and elementalDamageOverTime here
                    if (enemyProjectile.shootingEnemyData != null && enemyProjectile.shootingEnemyData.stats != null)
                    {
                        enemyProjectile.elementalPotency = enemyProjectile.shootingEnemyData.stats.elementalPotency;
                        enemyProjectile.elementalDamageOverTime = enemyProjectile.shootingEnemyData.stats.elementalDamageOverTime;
                    }

                   // UnityEngine.Debug.Log($"Projectile fired with Elemental Potency: {enemyProjectile.shootingEnemyData.stats.elementalPotency} and DOT: {enemyProjectile.shootingEnemyData.stats.elementalDamageOverTime}");
                    projectile.SetActive(true);
                }
            }
            else
            {
                UnityEngine.Debug.LogError("Prefab is null. Make sure it's assigned in projectilePoolData.");
            }
        }
    }
}
