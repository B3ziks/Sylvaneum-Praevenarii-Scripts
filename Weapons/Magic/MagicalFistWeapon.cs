using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random; // Use UnityEngine's Random class

public class MagicalFistWeapon : WeaponBase
{
    public PoolObjectData magicalFistPrefab; // Assign this in the inspector

    public override void Attack()
    {
        // Find a random enemy near the character to target
        Enemy targetEnemy = FindRandomEnemyNearCharacter();

        if (targetEnemy != null)
        {
            // Calculate a position above the enemy
            Vector3 strikePosition = targetEnemy.transform.position + Vector3.up * 3.0f; // 3 units above the enemy

            // Spawn the magical fist at the calculated position
            SpawnMagicalFist(strikePosition);
        }
    }

    private void SpawnMagicalFist(Vector3 position)
    {
        GameObject magicalFistGO = poolManager.GetObject(magicalFistPrefab);
        magicalFistGO.transform.position = position;
        magicalFistGO.SetActive(true);

        // Initialize the magical fist at the position
        MagicalFist magicalFist = magicalFistGO.GetComponent<MagicalFist>();
        if (magicalFist != null)
        {
            magicalFist.SetStats(this);
            magicalFist.Initialize(position); // Pass the strike position
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