using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JavelinWeapon : WeaponBase
{
    [SerializeField] private PoolObjectData javelinPoolData;
    private bool isAttacking = false;

    public override void Attack()
    {
        if (!isAttacking)
        {
            StartCoroutine(ThrowJavelin());
        }
    }

    private bool isFirstJavelin = true;

    private IEnumerator ThrowJavelin()
    {
        isAttacking = true;

        if (isFirstJavelin)
        {
            // If it's the first knife, set it to inactive
            GameObject firstJavelin = SpawnProjectile(javelinPoolData, transform.position);
            firstJavelin.SetActive(false);
            isFirstJavelin = false;
        }
        else
        {
            // For subsequent knives, set them to active
            GameObject javelin = SpawnProjectile(javelinPoolData, transform.position);
            javelin.SetActive(true);
        }

        yield return new WaitForSeconds(weaponData.stats.timeToAttack);
        isAttacking = false;
    }
}