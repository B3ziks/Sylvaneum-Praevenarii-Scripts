using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodInterventionEffect : MonoBehaviour
{
    public GameObject effectIndicator; // Assign the child GameObject that indicates the effect is active

    // Stat multipliers
    public float armorMultiplier = 2f;
    public float damageMultiplier = 2f;
    public float moveSpeedMultiplier = 1.5f;

    private Enemy enemyComponent;
    private int originalArmor;
    private int originalDamage;
    private float originalMoveSpeed;

    private void Awake()
    {
        enemyComponent = GetComponent<Enemy>();

    }

    public void StorageOriginalValues()
    {
        // Store original stats
        originalArmor = enemyComponent.stats.armor;
        originalDamage = enemyComponent.stats.damage;
        originalMoveSpeed = enemyComponent.stats.moveSpeed;
    }
    public void ActivateEffect()
    {
        // Activate the effect indicator
        effectIndicator.SetActive(true);

        // Increase stats
        enemyComponent.stats.armor = Mathf.RoundToInt(originalArmor * armorMultiplier);
        enemyComponent.stats.damage = Mathf.RoundToInt(originalDamage * damageMultiplier);
        enemyComponent.stats.moveSpeed *= moveSpeedMultiplier;
    }

    public void DeactivateEffect()
    {
        // Deactivate the effect indicator
        effectIndicator.SetActive(false);

        // Reset stats
        enemyComponent.stats.armor = originalArmor;
        enemyComponent.stats.damage = originalDamage;
        enemyComponent.stats.moveSpeed = originalMoveSpeed;
    }
}