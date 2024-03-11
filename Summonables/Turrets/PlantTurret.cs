using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantTurret : MonoBehaviour
{
    private IDamageable currentTarget;
    public PoolObjectData projectilePoolData;
    public Transform turretBarrelTransform;
    public float attackRange = 5f;
    // Remove the hardcoded fireRate declaration
    private float fireCountdown;
    PoolManager poolManager;
    public WeaponBase weaponBaseReference;
    //
    public Transform playerTransform; // Drag and drop your player object here in the Inspector
    public float maxDistanceFromPlayer = 10f; // Set this to whatever distance you consider "too far"
    public Vector3 offsetFromPlayer = new Vector3(2f, 2f, 0f); // Offset so turret doesn't spawn directly on top of the player


    private void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();

        CircleCollider2D detectionCollider = gameObject.AddComponent<CircleCollider2D>();
        detectionCollider.radius = attackRange;
        detectionCollider.isTrigger = true;

        // Access timeToAttack through weaponData.stats
        fireCountdown = weaponBaseReference.weaponData.stats.timeToAttack;

        if (playerTransform == null) // Safety check in case you forgot to assign the player's transform
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform; // This assumes you have a PlayerMove component on your player. Adjust as necessary.
        }
    }

    private void Update()
    {
        if (currentTarget == null || ((UnityEngine.Component)currentTarget).gameObject == null)
        {
            currentTarget = null;
            TeleportToPlayerIfTooFar();
            return;
        }

        AimAtTarget(currentTarget);
        fireCountdown -= Time.deltaTime;
        if (fireCountdown <= 0f)
        {
            ShootAtTarget(currentTarget);
            // Reset fireCountdown using the weaponBaseReference's timeToAttack
            fireCountdown = weaponBaseReference.weaponData.stats.timeToAttack;
        }
        TeleportToPlayerIfTooFar();

    }

    private void AimAtTarget(IDamageable target)
    {
        Vector2 direction = ((Vector2)((UnityEngine.Component)target).transform.position - (Vector2)turretBarrelTransform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        turretBarrelTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ShootAtTarget(IDamageable target)
    {
        GameObject bulletGO = SpawnProjectileFromPool(projectilePoolData, turretBarrelTransform.position, target);
        PlantTurretProjectile bullet = bulletGO.GetComponent<PlantTurretProjectile>();
        Vector2 direction = ((UnityEngine.Component)target).transform.position - turretBarrelTransform.position;
        bullet.SetDirection(direction.x, direction.y);
    }

    private GameObject SpawnProjectileFromPool(PoolObjectData poolObjectData, Vector3 position, IDamageable target)
    {
        GameObject projectileGO = poolManager.GetObject(poolObjectData);
        projectileGO.transform.position = position;
        PlantTurretProjectile projectile = projectileGO.GetComponent<PlantTurretProjectile>();
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

    void TeleportToPlayerIfTooFar()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > maxDistanceFromPlayer)
        {
            // Find a new position around the player that's not overlapping with other turrets or obstacles
            Vector2 newPosition = FindNewPositionAroundPlayer();
            if (newPosition != Vector2.zero)
            {
                transform.position = newPosition;
            }
            else
            {
                // Handle the case where a valid position isn't found
                // You could force a position, wait until the next frame and try again, etc.
                UnityEngine.Debug.LogError("Failed to teleport turret to a new valid position.");
            }
        }
    }


    Vector2 FindNewPositionAroundPlayer()
    {
        PlantTurretSpawner spawner = weaponBaseReference as PlantTurretSpawner;
        if (spawner != null)
        {
            return spawner.CalculateSpawnPositionAroundPlayer();
        }
        return Vector2.zero;
    }
    //gizmos
    void OnDrawGizmosSelected()
    {
        // Draw a red wire sphere representing the attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}