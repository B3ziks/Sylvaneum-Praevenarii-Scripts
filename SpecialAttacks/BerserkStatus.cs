using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkStatus : MonoBehaviour
{
    public GameObject effectIndicator; // Assign the child GameObject that indicates the effect is active

    // Stat multipliers
    public float armorDecreaseFactor = 0.5f; // Decrease armor by 50%
    public float damageMultiplier = 1.5f; // Increase damage by 50%
    public float attackSpeedMultiplier = 2f; // Double the attack speed

    private Enemy enemyComponent;
    private int originalArmor;
    private int originalDamage;
    private float originalAttackSpeed;

    private void Awake()
    {
        enemyComponent = GetComponent<Enemy>();
    }

    public void StoreOriginalValues()
    {
        // Store original stats
        originalArmor = enemyComponent.stats.armor;
        originalDamage = enemyComponent.stats.damage;
        originalAttackSpeed = enemyComponent.stats.timeToAttack;
    }

    public void ActivateEffect()
    {
        // Activate the effect indicator
        effectIndicator.SetActive(true);

        // Modify stats for berserk status
        enemyComponent.stats.armor = Mathf.RoundToInt(originalArmor * armorDecreaseFactor);
        enemyComponent.stats.damage = Mathf.RoundToInt(originalDamage * damageMultiplier);
        enemyComponent.stats.timeToAttack *= attackSpeedMultiplier;
    }

    public void DeactivateEffect()
    {
        // Deactivate the effect indicator
        effectIndicator.SetActive(false);

        // Reset stats to original values
        enemyComponent.stats.armor = originalArmor;
        enemyComponent.stats.damage = originalDamage;
        enemyComponent.stats.timeToAttack = originalAttackSpeed;
    }
}