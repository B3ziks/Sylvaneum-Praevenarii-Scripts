using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterSpiritTotem : MonoBehaviour
{
    public PoolObjectData projectilePoolData;
    public WeaponBase weaponBaseReference;
    public float attackRadius = 5f;
    public int numberOfProjectiles = 3;
    private PoolManager poolManager;
    private Animator animator;
    private bool isPossessed = false;
    private float possessedTimer;
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
        animator = GetComponent<Animator>();

        possessedTimer = weaponBaseReference.weaponData.stats.timeToAttack * 5;
        attackTimer = weaponBaseReference.weaponData.stats.timeToAttack;
    }

    private void Update()
    {
        // Handle the possessed state timing
        possessedTimer -= Time.deltaTime;
        if (possessedTimer <= 0)
        {
            isPossessed = !isPossessed; // Toggle the possessed state
            animator.SetBool("isPossessed", isPossessed);
            possessedTimer = weaponBaseReference.weaponData.stats.timeToAttack * 5;
        }

        // Handle the attack timing
        if (isPossessed)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0)
            {
                Attack();
                attackTimer = weaponBaseReference.weaponData.stats.timeToAttack;
            }
        }
        TeleportToPlayerIfTooFar();

    }

    private void Attack()
    {
        for (int i = 0; i < numberOfProjectiles; i++)
        {
            SpawnProjectile();
        }
    }

    private void SpawnProjectile()
    {
        float angle = UnityEngine.Random.Range(0, 360);
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        Vector2 spawnPosition = (Vector2)transform.position + direction * attackRadius;

        GameObject projectileObject = poolManager.GetObject(projectilePoolData);
        projectileObject.transform.position = spawnPosition;

        Projectile projectileComponent = projectileObject.GetComponent<Projectile>();
        if (projectileComponent != null)
        {
            projectileComponent.SetStats(weaponBaseReference);
            projectileComponent.SetDirection(direction.x, direction.y);
        }
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
        HunterSpiritTotemSpawner spawner = weaponBaseReference as HunterSpiritTotemSpawner;
        if (spawner != null)
        {
            return spawner.CalculateSpawnPositionAroundPlayer();
        }
        return Vector2.zero;
    }
}