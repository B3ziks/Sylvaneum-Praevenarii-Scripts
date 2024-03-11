using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class GunTurret : TurretBase
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
        // Specific implementation for GunTurret to shoot at the target
        GameObject bulletGO = SpawnProjectileFromPool(projectilePoolData, turretBarrelTransform.position, target);
        TurretProjectile bullet = bulletGO.GetComponent<TurretProjectile>();
        Vector2 direction = ((UnityEngine.Component)target).transform.position - turretBarrelTransform.position;
        bullet.SetDirection(direction.x, direction.y);
        bullet.SetStats(weaponBaseReference);
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
        if (enemy != null)
        {
            if (currentTarget == null)
            {
                currentTarget = enemy;
            }
            else
            {
                // If there's another target closer than the current target, switch targets
                float distanceToCurrentTarget = Vector2.Distance(transform.position, ((UnityEngine.Component)currentTarget).transform.position);
                float distanceToNewTarget = Vector2.Distance(transform.position, col.transform.position);
                if (distanceToNewTarget < distanceToCurrentTarget)
                {
                    currentTarget = enemy;
                }
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



    protected override Vector2 CalculateSpawnPositionAroundPlayer()
    {
        // Specific implementation for GunTurret to calculate a new position around the player
        GunTurretSpawner spawner = weaponBaseReference as GunTurretSpawner;
        if (spawner != null)
        {
            return spawner.CalculateSpawnPositionAroundPlayer();
        }
        return Vector2.zero;
    }

    protected override bool IsValidTarget(IDamageable target)
    {
        // Specific validation for GunTurret
        return target != null && ((UnityEngine.Component)target).gameObject != null;
    }

}