using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBombProjectile : Projectile
{
    public PoolObjectData flameEffectPoolData;
    public float clusterRadius =4f;
    private PoolManager poolManager;

    protected void Awake()
    {
        poolManager = FindObjectOfType<PoolManager>();
    }
    // This method is triggered when the projectile collides with something
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Obstacle"))
        {
            Ignite();
        }
    }
    public void SetPoolManager(PoolManager poolManager)
    {
        this.poolManager = poolManager;
    }
    private void Ignite()
    {
        // Damage and post damage to enemies in the central explosion
        ApplyAreaDamage(transform.position, radius);

        // Define the grid size and offsets based on the clusterRadius
        int gridSize = 2; // Since we want to spawn 4 flames, a 2x2 grid is sufficient
        float offsetDistance = clusterRadius / gridSize;

        // Calculate offsets for the four directions based on grid
        Vector3[] offsets = new Vector3[]
        {
        new Vector3(offsetDistance, offsetDistance, 0f),   // Top Right
        new Vector3(-offsetDistance, offsetDistance, 0f),  // Top Left
        new Vector3(-offsetDistance, -offsetDistance, 0f), // Bottom Left
        new Vector3(offsetDistance, -offsetDistance, 0f)   // Bottom Right
        };

        // Spawn the flames using the offsets to position them correctly
        foreach (Vector3 offset in offsets)
        {
            Vector3 spawnPosition = transform.position + offset;
            SpawnFlameEffect(spawnPosition);
        }

        // Deactivate this projectile
        gameObject.SetActive(false);
    }

    private void ApplyAreaDamage(Vector3 position, float radius)
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(position, radius);
        foreach (Collider2D hitCollider in hitColliders)
        {
            IDamageable damageable = hitCollider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                PostDamage(damage, position); // Assuming damage is dealt at the projectile's position

            }
        }
    }

    private void SpawnFlameEffect(Vector3 position)
    {
        GameObject flameEffect = poolManager.GetObject(flameEffectPoolData);
        if (flameEffect)
        {
            // Set the position and make sure it is active
            flameEffect.transform.position = position;
            flameEffect.SetActive(true);

            // If your flame effect has any kind of script that automatically sets its position,
            // you may need to disable that script here right after setting the position.
        }
        else
        {
            Debug.LogError("Failed to spawn flame effect.");
        }
    }
    public void PostDamage(int damage, Vector3 targetPosition)
    {
        Color messageColor = GetMessageColor(weapon.weaponData.stats.elementType); // Use weaponData from the base Projectile class
        MessageSystem.instance.PostMessage(damage.ToString(), targetPosition, messageColor);
    }
}