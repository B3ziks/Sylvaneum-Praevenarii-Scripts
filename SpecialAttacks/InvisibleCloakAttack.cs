using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Invisible Cloak Attack")]
public class InvisibleCloakAttack : SpecialAttack
{
    public float invisibilityDuration = 2.0f; // Duration of invisibility in seconds
    public float teleportDistance = 10.0f; // Distance to teleport

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (executor == null)
        {
            Debug.LogError("Executor is not set!");
            return;
        }

        // Start coroutine to handle the attack
        executor.GetMonoBehaviour().StartCoroutine(InvisibleCloakSequence(executor));
    }

    private IEnumerator InvisibleCloakSequence(ISpecialAttackExecutor executor)
    {
        Enemy enemyComponent = executor.GetMonoBehaviour().GetComponent<Enemy>();
        if (enemyComponent == null)
        {
            Debug.LogError("Enemy component not found on executor!");
            yield break;
        }

        // Make boss invisible and invulnerable
        SetVisibilityAndCollider(executor, false);
        int originalArmor = enemyComponent.stats.armor;
        enemyComponent.stats.armor = 999;

        yield return new WaitForSeconds(invisibilityDuration);

        // Teleport to a random direction
        TeleportRandomly(executor);

        // Restore original state
        SetVisibilityAndCollider(executor, true);
        enemyComponent.stats.armor = originalArmor;
    }

    private void SetVisibilityAndCollider(ISpecialAttackExecutor executor, bool state)
    {
        // Get all SpriteRenderer components in the boss and its children
        var spriteRenderers = executor.GetMonoBehaviour().GetComponentsInChildren<SpriteRenderer>();

        foreach (var spriteRenderer in spriteRenderers)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = state;
            }
        }

        var collider2D = executor.GetMonoBehaviour().GetComponent<BoxCollider2D>();
        if (collider2D != null)
        {
            collider2D.enabled = state;
        }
    }

    private void TeleportRandomly(ISpecialAttackExecutor executor)
    {
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 newPosition = executor.GetMonoBehaviour().transform.position + (Vector3)(randomDirection * teleportDistance);
        executor.GetMonoBehaviour().transform.position = newPosition;
    }
}