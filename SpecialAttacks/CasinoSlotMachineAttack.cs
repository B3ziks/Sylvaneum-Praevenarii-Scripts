using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Casino Slot Machine Attack")]
public class CasinoSlotMachineAttack : SpecialAttack
{
    public PoolObjectData slotMachinePoolData;  // Data for spawning the slot machine
    public PoolObjectData projectilePoolData;   // Data for spawning projectiles
    public float attackDuration = 2.0f;         // Duration of attack in seconds
    public float projectileSpeed = 10.0f;       // Speed of projectiles
    public float timeBetweenProjectiles = 0.5f; // Time between each projectile spawn

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (executor == null)
        {
            Debug.LogError("Executor is not set!");
            return;
        }

        // Start coroutine to handle the attack sequence
        executor.GetMonoBehaviour().StartCoroutine(CasinoSlotMachineSequence(executor, poolManager));
    }

    private IEnumerator CasinoSlotMachineSequence(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        Transform playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (playerTransform == null)
        {
            Debug.LogError("Player not found!");
            yield break;
        }

        // Spawn the slot machine
        GameObject slotMachine = poolManager.GetObject(slotMachinePoolData);
        if (slotMachine == null)
        {
            Debug.LogError("Slot machine object not available in pool!");
            yield break;
        }

        // Position the slot machine near the player or at a predefined location
        // Adjust the position as needed for your game's design
        slotMachine.transform.position = playerTransform.position + new Vector3(2, 0, 0);
        slotMachine.SetActive(true);

        float timer = 0f;
        while (timer < attackDuration)
        {
            // Make the slot machine shoot projectiles at the player
            SpawnProjectile(poolManager, slotMachine.transform.position, playerTransform.position);
            timer += timeBetweenProjectiles;
            yield return new WaitForSeconds(timeBetweenProjectiles);
        }

        // Optionally, deactivate the slot machine after the attack
        slotMachine.SetActive(false);
    }

    private void SpawnProjectile(PoolManager poolManager, Vector3 startPosition, Vector3 targetPosition)
    {
        GameObject projectileGO = poolManager.GetObject(projectilePoolData);
        if (projectileGO == null) return;

        projectileGO.transform.position = startPosition;
        Vector2 direction = (targetPosition - startPosition).normalized;

        EnemyProjectile projectileScript = projectileGO.GetComponent<EnemyProjectile>();
        if (projectileScript != null)
        {

            Rigidbody2D projectileRigidbody = projectileGO.GetComponent<Rigidbody2D>();
            if (projectileRigidbody != null)
            {
                projectileRigidbody.velocity = direction * projectileSpeed;
            }
        }

        projectileGO.SetActive(true);
    }
}