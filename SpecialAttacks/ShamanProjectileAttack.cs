using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/ShamanProjectileAttack")]
public class ShamanProjectileAttack : SpecialAttack
{
    public PoolObjectData projectilePoolData; // Assign this in the inspector
    public float attackDelay = 1.0f; // Delay between attacks

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        // Start the routine to spawn projectiles
        executor.GetMonoBehaviour().StartCoroutine(SpawnProjectileRoutine(executor, poolManager));
    }

    private IEnumerator SpawnProjectileRoutine(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        // Assuming you want to spawn a set number of projectiles, not infinitely
        int numberOfProjectiles = 3; // For example, spawn 3 projectiles

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            SpawnProjectile(executor, poolManager);
            yield return new WaitForSeconds(attackDelay); // Wait for the delay between attacks
        }
    }

    private void SpawnProjectile(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        GameObject projectile = poolManager.GetObject(projectilePoolData);
        if (projectile != null)
        {
            ShamanCreatureProjectile shamanProjectile = projectile.GetComponent<ShamanCreatureProjectile>();
            if (shamanProjectile != null)
            {
                // Set the player as the target
                Transform playerTransform = executor.GetPlayerTransform();
                shamanProjectile.Initialize(playerTransform);
                projectile.transform.position = executor.transform.position; // Or you can set it to a specific spawn point
                projectile.SetActive(true);
            }
            else
            {
                Debug.LogError("ShamanCreatureProjectile component not found on the pooled object!");
            }
        }
        else
        {
            Debug.LogError("Projectile could not be spawned from the pool.");
        }
    }
}