using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Special Attacks/Forest Sadness Attack")]
public class ForestSadnessAttack : SpecialAttack
{
    private EnemyData enemyData;

    // Reference to the rain particle effect prefab
    public GameObject rainParticleEffectPrefab;

    // Buff properties
    public float armorMultiplier = 1.5f;
    public float damageMultiplier = 1.5f;
    public float moveSpeedMultiplier = 1.2f;
    public float effectDuration = 10.0f; // Duration of the Forest Sadness effect
    private int originalDamage;
    private int originalArmor;
    private float originalMoveSpeed;
    public override void ExecuteAttack(ISpecialAttackExecutor executor, PoolManager poolManager)
    {
        // Apply rain effect
        ActivateRainEffect();

        // Apply stat buffs
        ApplyStatBuffs(executor);
    }

    private void ActivateRainEffect()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && rainParticleEffectPrefab != null)
        {
            Vector3 offset = new Vector3(0, 5f, 0);
            GameObject rainEffectInstance = Instantiate(rainParticleEffectPrefab, player.transform.position + offset, Quaternion.identity, player.transform);
            Debug.Log("Rain effect instantiated at " + rainEffectInstance.transform.position);
            rainEffectInstance.SetActive(true);
        }
        else
        {
            Debug.LogError("Player or Rain Particle Effect Prefab not found!");
        }
    }

    private void ApplyStatBuffs(ISpecialAttackExecutor executor)
    {
        Enemy enemyComponent = executor.GetMonoBehaviour().GetComponent<Enemy>();
        if (enemyComponent != null)
        {
            executor.GetMonoBehaviour().StartCoroutine(ApplyBuffEffect(enemyComponent));
        }
        else
        {
            Debug.LogError("Enemy component not found on the executor!");
        }
    }

    private IEnumerator ApplyBuffEffect(Enemy enemyComponent)
    {
        StoreOriginalValues(enemyComponent);
        ActivateEffect(enemyComponent);

        yield return new WaitForSeconds(effectDuration);

        DeactivateEffect(enemyComponent);
    }

    private void StoreOriginalValues(Enemy enemy)
    {
            originalArmor = enemy.stats.armor;
            originalDamage = enemy.stats.damage;
            originalMoveSpeed = enemy.stats.moveSpeed;
    }

    private void ActivateEffect(Enemy enemy)
    {
        enemy.stats.armor = Mathf.RoundToInt(originalArmor * armorMultiplier);
        enemy.stats.damage = Mathf.RoundToInt(originalDamage * damageMultiplier);
        enemy.stats.moveSpeed *= moveSpeedMultiplier;
    }

    private void DeactivateEffect(Enemy enemy)
    {
        enemy.stats.armor = originalArmor;
        enemy.stats.damage = originalDamage;
        enemy.stats.moveSpeed = originalMoveSpeed;
    }
}