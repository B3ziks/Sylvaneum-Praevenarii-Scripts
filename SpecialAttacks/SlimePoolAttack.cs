using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Slime Pool Attack")]
public class SlimePoolAttack : SpecialAttack
{
    public float poolDuration = 3f;  // Duration of the slime pool state

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        // Start the slime pool coroutine on the executor's MonoBehaviour
        executor.GetMonoBehaviour().StartCoroutine(SlimePoolCoroutine(executor));
    }

    private IEnumerator SlimePoolCoroutine(ISpecialAttackExecutor executor)
    {
        // Access the Enemy component from the executor
        Enemy enemyComponent = executor.GetMonoBehaviour().GetComponent<Enemy>();
        if (enemyComponent == null)
        {
            Debug.LogError("Enemy component not found on executor!");
            yield break;
        }

        Animator animator = executor.GetMonoBehaviour().GetComponentInChildren<Animator>();
        int originalArmor = enemyComponent.stats.armor;

        // Trigger slime pool animation
        animator.SetBool("isSpecialAttacking", true);

        // Make boss immune to damage
        enemyComponent.stats.armor = 999;

        // Wait for the pool duration
        yield return new WaitForSeconds(poolDuration);

        // Revert back to original state
        animator.SetBool("isSpecialAttacking", false);
        enemyComponent.stats.armor = originalArmor;
    }

}