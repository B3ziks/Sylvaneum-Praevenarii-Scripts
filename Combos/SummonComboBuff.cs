using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonComboBuff : MonoBehaviour
{
    public WeaponBase weaponBaseReference; // Reference to the weapon base for stats

    private int originalDamage;
    private float originalAttackSpeed;
    private int updatedDamage;
    public GameObject buffEffect; // Reference to the buff effect GameObject

    public void SetWeaponBaseReference(WeaponBase weaponBase)
    {
        weaponBaseReference = weaponBase;
    }
    private void Start()
    {
        if (weaponBaseReference != null)
        {
            // Initialize original stats from weapon base reference
            originalDamage = weaponBaseReference.weaponData.stats.damage;
            originalAttackSpeed = weaponBaseReference.weaponData.stats.timeToAttack;
        }
        else
        {
            Debug.LogError("WeaponBaseReference is not set in SummonComboBuff");
        }

        // Ensure the buff effect is initially disabled
        if (buffEffect != null)
        {
            buffEffect.SetActive(false);
        }
        else
        {
            Debug.LogError("BuffEffect is not set in SummonComboBuff");
        }
    }

    public void ApplyBuff(float damageMultiplier, float attackSpeedMultiplier, float duration)
    {
        // Activate the buff effect
        if (buffEffect != null)
        {
            buffEffect.SetActive(true);
        }

        int damage = weaponBaseReference.weaponData.stats.damage;
        float multipliedDamage = damage * damageMultiplier;
        updatedDamage = Mathf.RoundToInt(multipliedDamage);
        weaponBaseReference.weaponData.stats.damage = updatedDamage;

        weaponBaseReference.weaponData.stats.timeToAttack *= attackSpeedMultiplier;

        // Start coroutine to revert the buff after duration
        StartCoroutine(RevertBuff(duration));
    }

    private IEnumerator RevertBuff(float duration)
    {
        yield return new WaitForSeconds(duration);

        // Revert the stats back to original
        weaponBaseReference.weaponData.stats.damage = originalDamage;
        weaponBaseReference.weaponData.stats.timeToAttack = originalAttackSpeed;

        // Deactivate the buff effect
        if (buffEffect != null)
        {
            buffEffect.SetActive(false);
        }
    }
}
