using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class SniperTurret : TurretBase
{
  
    protected override void Start()
    {
        base.Start();
        // Additional start logic specific to GunTurret, if any
    }

    protected override void Update()
    {
        base.Update();
        // Additional update logic specific to GunTurret, if any
    }


    protected override void ShootAtTarget(IDamageable target)
    {
        GameObject bulletGO = SpawnProjectileFromPool(projectilePoolData, turretBarrelTransform.position, target);
        SniperTurretProjectile bullet = bulletGO.GetComponent<SniperTurretProjectile>();
        Vector2 direction = ((UnityEngine.Component)target).transform.position - turretBarrelTransform.position;
        bullet.SetDirection(direction.x, direction.y);
    }

    private GameObject SpawnProjectileFromPool(PoolObjectData poolObjectData, Vector3 position, IDamageable target)
    {
        GameObject projectileGO = poolManager.GetObject(poolObjectData);
        projectileGO.transform.position = position;
        TurretProjectile projectile = projectileGO.GetComponent<TurretProjectile>();
        Vector2 direction = ((UnityEngine.Component)target).transform.position - position;
        projectile.SetDirection(direction.x, direction.y);
        projectile.SetStats(weaponBaseReference);
        return projectileGO;
    }
    public void SetPoolManager(PoolManager poolManager)
    {
        this.poolManager = poolManager;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        IDamageable enemy = col.GetComponent<IDamageable>();
        Enemy enemyData = col.GetComponent<Enemy>(); // Assuming Enemy script has the data

        if (enemy != null && (enemyData.enemyData.isElite || enemyData.enemyData.isBoss))
        {
            if (currentTarget == null || IsCloserThanCurrentTarget(col.transform))
            {
                currentTarget = enemy;
            }
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.GetComponent<IDamageable>() == currentTarget)
        {
            currentTarget = null;
        }
    }

    private void FindNewTarget()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        float closestDistance = float.MaxValue;
        IDamageable closestTarget = null;

        foreach (var enemy in enemies)
        {
            if (enemy.enemyData.isElite || enemy.enemyData.isBoss)
            {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = enemy;
                }
            }
        }

        currentTarget = closestTarget;
    }
    protected override bool IsValidTarget(IDamageable target)
    {
        if (target == null) return false;
        Enemy enemyData = ((UnityEngine.Component)target).GetComponent<Enemy>(); // Check if enemyData exists
        return enemyData != null && (enemyData.enemyData.isElite || enemyData.enemyData.isBoss);
    }

    private bool IsCloserThanCurrentTarget(Transform potentialTarget)
    {
        if (currentTarget == null) return true;
        float distanceToCurrentTarget = Vector2.Distance(transform.position, ((UnityEngine.Component)currentTarget).transform.position);
        float distanceToPotentialTarget = Vector2.Distance(transform.position, potentialTarget.position);
        return distanceToPotentialTarget < distanceToCurrentTarget;
    }

    protected override Vector2 CalculateSpawnPositionAroundPlayer()
    {
        // Specific implementation for GunTurret to calculate a new position around the player
        SniperTurretSpawner spawner = weaponBaseReference as SniperTurretSpawner;
        if (spawner != null)
        {
            return spawner.CalculateSpawnPositionAroundPlayer();
        }
        return Vector2.zero;
    }

}