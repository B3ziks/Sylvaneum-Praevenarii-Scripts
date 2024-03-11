using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Healing Spirits Attack")]
public class HealingSpiritsAttack : SpecialAttack
{
    public PoolObjectData healingSpiritPoolData;
    public float healingDuration;
    public int healingAmount;
    public float circleRadius = 1f;  // The radius of the circle around the boss
    public float rotationSpeed = 1f; // Speed at which the spirit circles the boss

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        if (poolManager == null)
        {
            Debug.LogError("PoolManager is not set!");
            return;
        }

        GameObject healingSpirit = poolManager.GetObject(healingSpiritPoolData);
        if (healingSpirit == null)
        {
            Debug.LogError("Healing spirit object not available in pool!");
            return;
        }

        healingSpirit.transform.position = executor.transform.position;
        healingSpirit.SetActive(true);

        executor.GetMonoBehaviour().StartCoroutine(HealExecutorOverTime(executor, healingDuration, healingAmount, healingSpirit));
    }

    private IEnumerator HealExecutorOverTime(ISpecialAttackExecutor executor, float duration, int healAmount, GameObject healingSpirit)
    {
        float elapsedTime = 0;
        float healTimer = 0;  // Separate timer for healing
        Enemy enemyComponent = executor.transform.GetComponent<Enemy>();
        float angle = 0;  // Start angle for circular motion

        while (elapsedTime < duration)
        {
            if (enemyComponent != null && healTimer <= 0)
            {
                enemyComponent.Heal(healAmount);
                healTimer = 1f;  // Reset heal timer
            }

            // Update the position of the healing spirit
            angle += rotationSpeed * Time.deltaTime;
            Vector3 offset = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * circleRadius;
            healingSpirit.transform.position = executor.transform.position + offset;

            elapsedTime += Time.deltaTime;
            healTimer -= Time.deltaTime;
            yield return null;
        }

        healingSpirit.GetComponent<PoolMember>()?.ReturnToPool();
    }
}
