using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;

public class FlamethrowerTurret : MonoBehaviour
{
    public PoolObjectData flamethrowerEffectPoolData;
    public float flamethrowerLength = 3f;
    private float damageCountdown;
    private PoolManager poolManager;
    public WeaponBase weaponBaseReference;
    private GameObject activeFlamethrowerEffect;

    public Transform targetEnemy; // The enemy to target.
    private IDamageable currentTarget;
    public Transform turretBarrelTransform;
    public float damageRadius = 5f; 
    public float attackRange = 5f; // Use the attack range to determine when enemies are within reach of the turret
     public Vector2 flamethrowerNozzleOffset;
    float estimatedGunLength = 0.71162548f; // Previously calculated x-offset
    private float lastKnownXDirection = 1f; // 1f for right, -1f for left


    // Teleport to player if too far
    public Transform playerTransform;
    public float maxDistanceFromPlayer = 10f;
    public Vector3 offsetFromPlayer = new Vector3(2f, 2f, 0f);

    private void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();
        damageCountdown = weaponBaseReference.weaponData.stats.timeToAttack;
        flamethrowerNozzleOffset.x += estimatedGunLength * 0.1f; // Adjusting by 5% for x
        flamethrowerNozzleOffset.y -= estimatedGunLength * -1f; // Adjusting by 10% for y
        CircleCollider2D detectionCollider = gameObject.AddComponent<CircleCollider2D>();
        detectionCollider.radius = attackRange;
        detectionCollider.isTrigger = true;

        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform;
        }
    }

    private void Update()
    {

        if (currentTarget == null || ((UnityEngine.Component)currentTarget).gameObject == null)
        {
            currentTarget = null;
            DeactivateFlameEffect();

            TeleportToPlayerIfTooFar();
            return;
        }
        AimAtTarget(currentTarget);
        damageCountdown -= Time.deltaTime;

        if (damageCountdown <= 0f)
        {
            SpawnFlamethrowerEffectTowardsEnemy(currentTarget);
            damageCountdown = weaponBaseReference.weaponData.stats.timeToAttack;
        }

        TeleportToPlayerIfTooFar();
    }

    private void AimAtTarget(IDamageable target)
    {
        SpriteRenderer spriteRenderer = turretBarrelTransform.GetComponent<SpriteRenderer>();
        Vector2 direction = ((Vector2)((UnityEngine.Component)target).transform.position - (Vector2)turretBarrelTransform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float bufferZone = 0.1f;

        if (Mathf.Abs(direction.x) < bufferZone) // Target is either directly above or below
        {
            direction.x = lastKnownXDirection; // Use last known horizontal direction
        }
        else
        {
            lastKnownXDirection = Mathf.Sign(direction.x); // Store the current horizontal direction
        }

        if (direction.x > bufferZone) // Target is to the right
        {
            spriteRenderer.flipX = false; // Ensure sprite is not flipped
            angle = Mathf.Clamp(angle, -90f, 90f); // Clamp the angle
            turretBarrelTransform.rotation = Quaternion.Euler(0, 0, angle);

            if (direction.y < 0) // Aiming downward to the right
            {
                turretBarrelTransform.localPosition = new Vector3(0.9f, -0.9f, turretBarrelTransform.localPosition.z);
            }
            else if (direction.y > 0) // Aiming upward to the right
            {
                turretBarrelTransform.localPosition = new Vector3(1f, -0.286f, turretBarrelTransform.localPosition.z);
            }
            else
            {
                // Reset position to default for right aiming
                turretBarrelTransform.localPosition = new Vector3(1.218f, turretBarrelTransform.localPosition.y, turretBarrelTransform.localPosition.z);
            }
        }
        else if (direction.x < -bufferZone) // Target is to the left
        {
            spriteRenderer.flipX = true; // Flip the sprite
                                         // Adjust the angle when flipped on the X-axis
            if (angle > 0.1)
            {
                angle = -180 + angle;
            }
            else
            {
                angle = 180 + angle;
            }
            turretBarrelTransform.rotation = Quaternion.Euler(0, 0, angle);

            if (direction.y < 0) // Aiming downward to the left
            {
                turretBarrelTransform.localPosition = new Vector3(0.6f, -0.76f, turretBarrelTransform.localPosition.z);
            }
            else
            {
                // Adjust the X position when aiming to the left
                turretBarrelTransform.localPosition = new Vector3(0.6f, turretBarrelTransform.localPosition.y, turretBarrelTransform.localPosition.z);
            }
        }
        // If it's within the buffer zone, don't change the sprite direction or position
    }


    private void SpawnFlamethrowerEffectTowardsEnemy(IDamageable target)
    {
        if (target == null)
        {
            DeactivateFlameEffect();
            return; // Exit if no target is specified
        }

        // If there's an active effect, return it to the pool
        if (activeFlamethrowerEffect != null)
        {
            PoolMember poolMemberComponent = activeFlamethrowerEffect.GetComponent<PoolMember>();
            if (poolMemberComponent != null)
            {
                poolMemberComponent.ReturnToPool();
            }
            activeFlamethrowerEffect = null;
        }

        Vector2 direction = ((Vector2)((UnityEngine.Component)target).transform.position) - (Vector2)turretBarrelTransform.position;
        direction = direction.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjusting the spawn position based on the turret's rotation
        Vector3 adjustedNozzleOffset = Quaternion.Euler(0, 0, angle) * flamethrowerNozzleOffset;
        Vector3 spawnPosition = turretBarrelTransform.position + (Vector3)(direction * (flamethrowerLength + estimatedGunLength)) + adjustedNozzleOffset;

        // Using object pooling for the flamethrower effect
        activeFlamethrowerEffect = poolManager.GetObject(flamethrowerEffectPoolData);
        activeFlamethrowerEffect.transform.position = spawnPosition;
        activeFlamethrowerEffect.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Applying damage to the affected enemies
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(spawnPosition, damageRadius);
        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                int damageValue = weaponBaseReference.weaponData.stats.damage;
                damageable.TakeDamage(damageValue);

                // Applying additional effects
                weaponBaseReference.ApplyAdditionalEffects(damageable, enemy.transform.position);

                // Post the damage message
                weaponBaseReference.PostDamage(damageValue, enemy.transform.position);
            }
        }
    }
    void DeactivateFlameEffect()
    {
        if (activeFlamethrowerEffect != null)
        {
            PoolMember poolMemberComponent = activeFlamethrowerEffect.GetComponent<PoolMember>();
            if (poolMemberComponent != null)
            {
                poolMemberComponent.ReturnToPool();
            }
            activeFlamethrowerEffect = null;
        }
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
            DeactivateFlameEffect();
            currentTarget = null;
        }
    }
    // Teleport the turret if it's too far from the player
    void TeleportToPlayerIfTooFar()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > maxDistanceFromPlayer)
        {
            Vector3 randomOffset = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), 0).normalized;
            transform.position = playerTransform.position + offsetFromPlayer + randomOffset;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}