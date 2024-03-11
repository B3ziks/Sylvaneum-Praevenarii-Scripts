using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/General Healing Shout Attack")]
public class GeneralHealingShoutAttack : SpecialAttack
{
    public float healingDuration;
    public int healingAmount;
    public float shoutRadius = 5f;  // The radius of the healing shout effect

    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        executor.GetMonoBehaviour().StartCoroutine(HealEnemiesInRadius(executor.transform.position, shoutRadius, healingDuration, healingAmount));
    }

    private IEnumerator HealEnemiesInRadius(Vector3 center, float radius, float duration, int healAmount)
    {
        float elapsedTime = 0;
        float healTimer = 0;  // Separate timer for healing

        while (elapsedTime < duration)
        {
            if (healTimer <= 0)
            {
                Collider[] hitColliders = Physics.OverlapSphere(center, radius);
                foreach (var hitCollider in hitColliders)
                {
                    Enemy enemy = hitCollider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.Heal(healAmount);
                    }
                }

                healTimer = 1f;  // Reset heal timer
            }

            elapsedTime += Time.deltaTime;
            healTimer -= Time.deltaTime;
            yield return null;
        }
    }
}