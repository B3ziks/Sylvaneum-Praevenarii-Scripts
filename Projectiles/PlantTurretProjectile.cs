using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantTurretProjectile : TurretProjectile
{
    [SerializeField]
    private PoolObjectData entanglingRootsPoolData; // Data for the entangling roots prefab

    public float rootsRadius; // Radius to spawn entangling roots
    public float rootsDuration; // Duration for which roots will be active

    private PoolManager poolManager;

    protected void Start()
    {
        poolManager = FindObjectOfType<PoolManager>();
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void SetStats(WeaponBase weaponBase)
    {
        base.SetStats(weaponBase);
        rootsRadius = weaponBase.weaponData.stats.explosionRadius; // Assuming you use the same stat for root radius
        rootsDuration = weaponBase.weaponData.stats.elementalPotency; // Assuming elemental potency is used for duration
    }

    protected override void DetectHits()
    {
        base.DetectHits(); // Call the base detection logic

        // Check for root spawning conditions
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackArea);
        foreach (Collider2D collider in hitColliders)
        {
            Enemy enemy = collider.GetComponent<Enemy>();
            if (enemy != null && !AlreadyHit(enemy))
            {
                enemy.Stun(rootsDuration);
                SpawnEntanglingRoots();
                break; // Ensure that roots are only spawned once per update
            }
        }
    }

    private void SpawnEntanglingRoots()
    {
        // Perform root spawning logic
        if (poolManager != null && entanglingRootsPoolData != null)
        {
            GameObject entanglingRoots = poolManager.GetObject(entanglingRootsPoolData);
            if (entanglingRoots)
            {
                entanglingRoots.transform.position = transform.position;
                entanglingRoots.transform.rotation = Quaternion.identity;
                entanglingRoots.transform.localScale = Vector3.one * (rootsRadius); // Scaling to the diameter
            }
            else
            {
                UnityEngine.Debug.LogError("Failed to get entangling roots from pool.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("PoolManager or entanglingRootsPoolData is not set.");
        }

        DeactivateProjectile();
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other); // Call the base method to handle other collisions

        // Specific behavior for the plant projectile when hitting an obstacle or enemy
        if (other.CompareTag("Obstacle") || other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Stun(rootsDuration);
            }
            SpawnEntanglingRoots();
        }
    }
}