using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StormTotem : MonoBehaviour
{
    public PoolObjectData projectilePoolData; // Data needed to get the projectile object from the pool
    public float damageRadius = 5f;
    public int numberOfProjectiles = 3;
    private PoolManager poolManager;
    public WeaponBase weaponBaseReference;

    // Variables for player distance checking
    public Transform playerTransform;
    public float maxDistanceFromPlayer = 10f;
    public Vector3 offsetFromPlayer = new Vector3(2f, 2f, 0f);

    private void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();
        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform;
        }
        StartCoroutine(SpawnProjectilesCoroutine());
    }

    private void Update()
    {
        TeleportToPlayerIfTooFar();
    }

    private IEnumerator SpawnProjectilesCoroutine()
    {
        while (true)
        {
            for (int i = 0; i < numberOfProjectiles; i++)
            {
                SpawnProjectile();
                yield return new WaitForSeconds(weaponBaseReference.weaponData.stats.timeToAttack / numberOfProjectiles);
            }
        }
    }

    private void SpawnProjectile()
    {
        float angle = UnityEngine.Random.Range(0, 360);
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        Vector2 spawnPosition = (Vector2)transform.position + direction * damageRadius;

        GameObject projectileObject = poolManager.GetObject(projectilePoolData);
        projectileObject.transform.position = spawnPosition;

        // Initialize the projectile component with stats from the weaponBaseReference
        Projectile projectileComponent = projectileObject.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.SetStats(weaponBaseReference); // This sets the projectile's speed, damage, etc.
            projectileComponent.SetDirection(direction.x, direction.y); // This sets the projectile's direction
        }
    }

   
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, damageRadius);
    }

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
                UnityEngine.Debug.LogWarning("Failed to find a new valid position for the totem!");
            }
        }
    }

    Vector2 FindNewPositionAroundPlayer()
    {
        StormTotemSpawner spawner = weaponBaseReference as StormTotemSpawner;
        if (spawner != null)
        {
            return spawner.CalculateSpawnPositionAroundPlayer();
        }
        return Vector2.zero;
    }
}