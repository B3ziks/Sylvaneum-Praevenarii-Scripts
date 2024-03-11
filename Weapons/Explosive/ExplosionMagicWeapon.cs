using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ExplosionMagicWeapon : WeaponBase
{
    public PoolObjectData explosionEffectPoolData; // Assign this in the inspector

    public override void Attack()
    {
        // Find a random enemy near the character to target
        Enemy targetEnemy = FindRandomEnemyNearCharacter();

        if (targetEnemy != null)
        {
            // Spawn the explosion effect at the enemy's position
            SpawnExplosionMagicStrike(targetEnemy.transform.position, targetEnemy);
        }
    }

    private void SpawnExplosionMagicStrike(Vector3 position, Enemy targetEnemy)
    {
        GameObject explosionMagicGO = poolManager.GetObject(explosionEffectPoolData);
        explosionMagicGO.transform.position = position;

        // Set the scale of the explosion effect based on the explosion radius
        float scaleMultiplier = weaponData.stats.explosionRadius;
        explosionMagicGO.transform.localScale = new Vector3(scaleMultiplier, scaleMultiplier, 1f);

        explosionMagicGO.SetActive(true);

        // Initialize the explosion strike with the target enemy and weapon stats
        ExplosionMagicStrike explosionStrike = explosionMagicGO.GetComponent<ExplosionMagicStrike>();
        if (explosionStrike != null)
        {
            explosionStrike.SetStats(this);
            explosionStrike.Initialize(targetEnemy);
        }
    }

    private Enemy FindRandomEnemyNearCharacter()
    {
        // Get all enemies within a certain range
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, 5f); // 5 units radius
        List<Enemy> nearbyEnemies = new List<Enemy>();

        foreach (var hitCollider in hitColliders)
        {
            Enemy enemy = hitCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                nearbyEnemies.Add(enemy);
            }
        }

        // Return a random enemy from the list
        if (nearbyEnemies.Count > 0)
        {
            int randomIndex = Random.Range(0, nearbyEnemies.Count);
            return nearbyEnemies[randomIndex];
        }

        return null; // No enemies found
    }
}