using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TurretBase : MonoBehaviour
{
    protected IDamageable currentTarget;
    public PoolObjectData projectilePoolData;
    public Transform turretBarrelTransform;
    public float attackRange = 8f;
    protected float fireCountdown;
    protected PoolManager poolManager;
    public WeaponBase weaponBaseReference;
    public Transform playerTransform;
    public float maxDistanceFromPlayer = 10f;
    public Vector3 offsetFromPlayer = new Vector3(2f, 2f, 0f);
    private float teleportCooldown = 1.5f;
    private float lastTeleportTime;

    protected abstract void ShootAtTarget(IDamageable target);
    protected abstract Vector2 CalculateSpawnPositionAroundPlayer();
    protected abstract bool IsValidTarget(IDamageable target);

    protected virtual void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();
        fireCountdown = weaponBaseReference.weaponData.stats.timeToAttack;
        attackRange = weaponBaseReference.weaponData.stats.attackRange;

        var detectionCollider = gameObject.AddComponent<CircleCollider2D>();
        detectionCollider.radius = attackRange;
        detectionCollider.isTrigger = true;

        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform;
        }
    }

    protected virtual void Update()
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
            fireCountdown = weaponBaseReference.weaponData.stats.timeToAttack;
        }
        TeleportToPlayerIfTooFar();
    }
    protected virtual void AimAtTarget(IDamageable target)
    {
        Vector2 direction = ((Vector2)((UnityEngine.Component)target).transform.position - (Vector2)turretBarrelTransform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        turretBarrelTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
    protected void TeleportToPlayerIfTooFar()
    {
        if (Time.time - lastTeleportTime < teleportCooldown) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > maxDistanceFromPlayer)
        {
            Vector2 newPosition = CalculateSpawnPositionAroundPlayer();
            if (newPosition != Vector2.zero)
            {
                transform.position = newPosition;
                lastTeleportTime = Time.time;
            }
            else
            {
                transform.position = playerTransform.position + offsetFromPlayer;
                lastTeleportTime = Time.time;
                Debug.LogWarning("Failed to teleport turret to a new valid position. Using fallback position.");
            }
        }
    }
    protected void FindNewTarget()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        float closestDistance = float.MaxValue;
        IDamageable closestTarget = null;

        foreach (var enemy in enemies)
        {
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = enemy;
                }
        }

        currentTarget = closestTarget;
    }
    // This method would likely be the same in all turrets
    protected bool IsCloserThanCurrentTarget(Transform potentialTarget, Transform currentTarget)
    {
        if (currentTarget == null) return true;
        float distanceToCurrentTarget = Vector2.Distance(transform.position, currentTarget.position);
        float distanceToPotentialTarget = Vector2.Distance(transform.position, potentialTarget.position);
        return distanceToPotentialTarget < distanceToCurrentTarget;
    }

    // Gizmo drawing for the attack range
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}