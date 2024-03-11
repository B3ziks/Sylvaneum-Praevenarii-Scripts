using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeteorTotem : MonoBehaviour
{
    public PoolObjectData meteorPoolData; // Assign this in the inspector
    public WeaponBase weaponBaseReference;
    public float detectionRadius = 5f;
    private PoolManager poolManager;
    private float attackTimer;
    //
    public Transform playerTransform;
    public float maxDistanceFromPlayer = 10f;
    public Vector3 offsetFromPlayer = new Vector3(2f, 2f, 0f);

    private void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = FindObjectOfType<PlayerMove>().transform;
        }
        poolManager = FindObjectOfType<PoolManager>();
        attackTimer = weaponBaseReference.weaponData.stats.timeToAttack;
    }

    private void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            Enemy targetEnemy = FindRandomEnemyNearTotem();
            if (targetEnemy != null)
            {
                SpawnMeteor(targetEnemy.transform.position);
            }
            attackTimer = weaponBaseReference.weaponData.stats.timeToAttack;
        }
        TeleportToPlayerIfTooFar();
    }

    private Enemy FindRandomEnemyNearTotem()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        List<Enemy> nearbyEnemies = new List<Enemy>();

        foreach (var hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                nearbyEnemies.Add(enemy);
            }
        }

        if (nearbyEnemies.Count > 0)
        {
            int randomIndex = Random.Range(0, nearbyEnemies.Count);
            return nearbyEnemies[randomIndex];
        }

        return null;
    }

    private void SpawnMeteor(Vector3 targetPosition)
    {
        GameObject meteorGO = poolManager.GetObject(meteorPoolData);
        meteorGO.transform.position = targetPosition + Vector3.up * 3.0f; // Position the meteor above the target
        meteorGO.SetActive(true);

        MeteorPlayer meteor = meteorGO.GetComponent<MeteorPlayer>();
        if (meteor != null)
        {
            meteor.SetStats(weaponBaseReference);
            meteor.Initialize(targetPosition); // Initialize the meteor with the target position
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
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
                UnityEngine.Debug.LogWarning("Failed to find a new valid position for the totem!");
            }
        }
    }

    Vector2 FindNewPositionAroundPlayer()
    {
        MeteorTotemSpawner spawner = weaponBaseReference as MeteorTotemSpawner;
        if (spawner != null)
        {
            return spawner.CalculateSpawnPositionAroundPlayer();
        }
        return Vector2.zero;
    }
}
