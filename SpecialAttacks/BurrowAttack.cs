using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Burrow Attack")]
public class BurrowAttack : SpecialAttack
{
    public float invisibilityDuration = 2.0f; // Duration of invisibility in seconds

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (executor == null)
        {
            Debug.LogError("Executor is not set!");
            return;
        }

        // Start coroutine to handle the attack
        executor.GetMonoBehaviour().StartCoroutine(BurrowSequence(executor));
    }

    private IEnumerator BurrowSequence(ISpecialAttackExecutor executor)
    {
        Enemy enemyComponent = executor.GetMonoBehaviour().GetComponent<Enemy>();
        if (enemyComponent == null)
        {
            Debug.LogError("Enemy component not found on executor!");
            yield break;
        }

        // Make enemy invisible and invulnerable
        SetVisibilityAndCollider(executor, false);
        int originalArmor = enemyComponent.stats.armor;
        enemyComponent.stats.armor = 999;

        yield return new WaitForSeconds(invisibilityDuration);

        // Teleport to the player's last known position
        TeleportToPlayer(executor);

        // Restore original state
        SetVisibilityAndCollider(executor, true);
        enemyComponent.stats.armor = originalArmor;
    }

    private void SetVisibilityAndCollider(ISpecialAttackExecutor executor, bool state)
    {
        var spriteRenderers = executor.GetMonoBehaviour().GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
                spriteRenderer.enabled = state;
        }

        var collider2D = executor.GetMonoBehaviour().GetComponent<BoxCollider2D>();
        if (collider2D != null)
            collider2D.enabled = state;
    }

    private void TeleportToPlayer(ISpecialAttackExecutor executor)
    {
        Transform playerTransform = executor.GetPlayerTransform();
        if (playerTransform != null)
        {
            // Get the player's position
            Vector3 playerPosition = playerTransform.position;

            // Define the range of the offset, for example, 1 unit
            float offsetRange = 5.0f;

            // Create a random offset within the range
            Vector3 offset = new Vector3(
               UnityEngine.Random.Range(-offsetRange, offsetRange),
                 UnityEngine.Random.Range(-offsetRange, offsetRange),
                0); // Assuming a 2D game

            // Apply the offset to the player's position
            executor.GetMonoBehaviour().transform.position = playerPosition + offset;
        }
        else
        {
            Debug.LogError("Player transform not found for teleportation!");
        }
    }
}