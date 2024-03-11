using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningTurret : MonoBehaviour
{
    private IDamageable currentTarget;
    public PoolObjectData projectilePoolData;
    public Transform turretBarrelTransform;
    public float attackRange = 5f;
    private float fireCountdown;
    PoolManager poolManager;
    public WeaponBase weaponBaseReference;

    public Transform playerTransform;
    public float maxDistanceFromPlayer = 10f;
    public Vector3 offsetFromPlayer = new Vector3(2f, 2f, 0f);

    public GameObject turretBase;
    public GameObject lightningTurretBarrelIdle;
    public GameObject lightningTurretBarrelCharging;
    private bool isCharging = false;
    private float chargeTime = 1f;
    public float lightningRange = 5f;
    public float damagePerSecond = 10f;

    private void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();

        CircleCollider2D detectionCollider = gameObject.AddComponent<CircleCollider2D>();
        detectionCollider.radius = attackRange;
        detectionCollider.isTrigger = true;

        fireCountdown = weaponBaseReference.weaponData.stats.timeToAttack;

        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform;
        }
    }

    private void Update()
    {
        // Check for target and teleport first
        if (currentTarget == null || ((UnityEngine.Component)currentTarget).gameObject == null)
        {
            currentTarget = null;
            TeleportToPlayerIfTooFar();
            return; // Exit the Update if no target
        }

        // Decrement the countdown.
        fireCountdown -= Time.deltaTime;

        if (fireCountdown <= 0f)
        {
            if (Vector2.Distance(turretBarrelTransform.position, ((UnityEngine.Component)currentTarget).transform.position) <= lightningRange)
            {
                StartCharging();
            }
        }
        TeleportToPlayerIfTooFar();
    }

    private void StartCharging()
    {
        if (!isCharging)
        {
            isCharging = true;
            lightningTurretBarrelIdle.SetActive(false);
            lightningTurretBarrelCharging.SetActive(true);

            StartCoroutine(ShootAfterCharge());
        }
    }

    private IEnumerator ShootAfterCharge()
    {
        yield return new WaitForSeconds(chargeTime);

        // Spawn a projectile after charging
        SpawnProjectileFromPool(projectilePoolData, turretBarrelTransform.position, currentTarget);


        lightningTurretBarrelIdle.SetActive(true);
        lightningTurretBarrelCharging.SetActive(false);
        isCharging = false;
        fireCountdown = weaponBaseReference.weaponData.stats.timeToAttack;
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
            // Instead of finding a new position, teleport turret closer to the player using an offset
            Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0).normalized;
            transform.position = playerTransform.position + offsetFromPlayer + randomOffset;
        }
    }

    Vector2 FindNewPositionAroundPlayer()
    {
        LightningTurretSpawner spawner = weaponBaseReference as LightningTurretSpawner;
        if (spawner != null)
        {
            return spawner.CalculateSpawnPositionAroundPlayer();
        }
        return Vector2.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private GameObject SpawnProjectileFromPool(PoolObjectData poolObjectData, Vector3 position, IDamageable target)
    {
        if (target == null || ((UnityEngine.Component)target).gameObject == null)
        {
            return null;  // Return null or handle the situation as per your requirement
        }

        Vector2 direction = ((UnityEngine.Component)target).transform.position - position;
        Vector3 offsetPosition = position + new Vector3(direction.x, direction.y, 0) * 0.5f; // Offset to avoid sticking inside the turret prefab
        GameObject projectileGO = poolManager.GetObject(poolObjectData);
        projectileGO.transform.position = offsetPosition;
        LightningTurretProjectile projectile = projectileGO.GetComponent<LightningTurretProjectile>();
        projectile.SetDirection(direction.x, direction.y);
        projectile.SetStats(weaponBaseReference);

        SpriteRenderer spriteRenderer = projectile.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            projectile.SetLineSprite(spriteRenderer.sprite);
        }

        return projectileGO;
    }
}