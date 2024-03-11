using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; // Use UnityEngine's Random class

public class LightningStrikeWeapon : WeaponBase
{
    public PoolObjectData lightningStrikePrefab; // Assign this in the inspector

    public override void Attack()
    {
        // Find a random enemy near the character to target
        Enemy targetEnemy = FindRandomEnemyNearCharacter();

        if (targetEnemy != null)
        {
            // Calculate a position above the enemy
            Vector3 strikePosition = targetEnemy.transform.position + Vector3.up * 2.0f; // 2 units above the enemy

            // Spawn the lightning strike at the calculated position
            SpawnLightningStrike(strikePosition, targetEnemy);
        }
    }

    private void SpawnLightningStrike(Vector3 position, Enemy targetEnemy)
    {
        GameObject lightningStrikeGO = poolManager.GetObject(lightningStrikePrefab);
        lightningStrikeGO.transform.position = position;
        lightningStrikeGO.SetActive(true);

        // Initialize the lightning strike with the target enemy and weapon stats
        LightningStrike lightningStrike = lightningStrikeGO.GetComponent<LightningStrike>();
        if (lightningStrike != null)
        {
            lightningStrike.SetStats(this);
            lightningStrike.Initialize(targetEnemy);
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