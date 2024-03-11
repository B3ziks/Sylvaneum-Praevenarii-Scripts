using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTotem : MonoBehaviour
{
    public PoolObjectData flamethrowerEffectPoolData;
    public float damageRadius = 5f;
    public float flamethrowerLength = 3f;
    private float damageCountdown;
    private PoolManager poolManager;
    public WeaponBase weaponBaseReference;
    private GameObject activeFlamethrowerEffect;
    //
    public Transform playerTransform; // Drag and drop your player object here in the Inspector
    public float maxDistanceFromPlayer = 10f; // Set this to whatever distance you consider "too far"
    public Vector3 offsetFromPlayer = new Vector3(2f, 2f, 0f); // Offset so turret doesn't spawn directly on top of the player

    private void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();
        damageCountdown = weaponBaseReference.weaponData.stats.timeToAttack;
        if (playerTransform == null) // Safety check in case you forgot to assign the player's transform
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform; // This assumes you have a PlayerMove component on your player. Adjust as necessary.
        }
    }

    private void Update()
    {
        damageCountdown -= Time.deltaTime;
        if (damageCountdown <= 0f)
        {
            SpawnFlamethrowerEffect();
            damageCountdown = weaponBaseReference.weaponData.stats.timeToAttack;
        }
        TeleportToPlayerIfTooFar();
    }

    private void SpawnFlamethrowerEffect()
    {
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

        float angle = UnityEngine.Random.Range(0, 360);
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        Vector2 spawnPosition = (Vector2)transform.position + direction * flamethrowerLength;

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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }
    //
    void TeleportToPlayerIfTooFar()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > maxDistanceFromPlayer)
        {
            Vector2 newPosition = FindNewPositionAroundPlayer();
            if (newPosition != Vector2.zero)
            {
                transform.position = newPosition;
            }
            else
            {
                // In case no valid position is found after several attempts, we can optionally destroy the turret or log a warning.
                UnityEngine.Debug.LogWarning("Failed to find a new valid position for the turret!");
            }
        }
    }

    Vector2 FindNewPositionAroundPlayer()
    {
        FireTotemSpawner spawner = weaponBaseReference as FireTotemSpawner;
        if (spawner != null)
        {
            return spawner.CalculateSpawnPositionAroundPlayer();
        }
        return Vector2.zero;
    }
}
